using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using IO.Juenger.GitLab.Model;
using Microsoft.Extensions.Logging;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.DataAccess.GitLab.Configs;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.DataAccess.GitLab.Providers
{
    internal class SprintInfoProvider : ISprintInfoProvider
    {
        private readonly IProjectApiProvider _projectApiProvider;
        private readonly IPaginationService _paginationService;
        private readonly ISprintProviderConfig _config;
        private readonly ILogger<SprintInfoProvider> _logger;

        public SprintInfoProvider(
            IProjectApiProvider projectApiProvider,
            IPaginationService paginationService, 
            ISprintProviderConfig config, 
            ILogger<SprintInfoProvider> logger)
        {
            _projectApiProvider = projectApiProvider ?? throw new ArgumentNullException(nameof(projectApiProvider));
            _paginationService = paginationService ?? throw new ArgumentNullException(nameof(paginationService));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<IReadOnlyList<SprintInfo>> GetAllSprintsAsync(string projectId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(projectId));
            }
            
            var sprintLabels = await _paginationService
                .BrowseAllAsync(page => 
                    _projectApiProvider
                        .ProjectApi
                        .GetProjectLabelsAsync(projectId, page, cancellationToken: ct))
                .ConfigureAwait(false);

            return sprintLabels
                .Where(FilterSprintLabel)
                .Select(label => 
                {
                    var (start, end) = GetSprintTimeFromLabelDescription(label);
                    
                    return new SprintInfo
                    {
                        Id = label.Id,
                        Name = label.Name,
                        StartTime = start,
                        EndTime = end
                    };
                })
                .OrderBy(spi => spi.StartTime)
                .ToList();
        }

        public async Task<(bool IsSuccess, SprintInfo SprintInfo)> TryGetCurrentSprintAsync(string projectId, CancellationToken ct = default)
        {
            var sprintInfos = await GetAllSprintsAsync(projectId, ct)
                .ConfigureAwait(false);

            var now = DateTime.UtcNow;
            var currentSprintInfo =
                sprintInfos.FirstOrDefault(sprintInfo => now >= sprintInfo.StartTime && now <= sprintInfo.EndTime);

            return currentSprintInfo == null ? (false, null) : (true, currentSprintInfo);
        }

        private bool FilterSprintLabel(Label label)
        {
            var rgx = new Regex(_config.SprintLabelPattern);
            var match = rgx.Match(label.Name);
            return match.Success;
        }
        
        private (DateTime Start, DateTime End) GetSprintTimeFromLabelDescription(Label label)
        {
            var rgx = new Regex(_config.SprintTimePattern);
            var matches = rgx.Matches(label.Description);

            var from = DateTime.MinValue;
            var to = DateTime.MinValue;
            
            foreach (Match match in matches)
            {
                if (!match.Success) continue;
                
                var split = match.Value.Split(" ");
                var time = DateTime.Parse(split[1]);
                    
                if (match.Value.ToLower().Contains("from"))
                {
                    @from = time;
                }
                else if (match.Value.ToLower().Contains("to"))
                {
                    to = time;
                }
            }

            return (from, to);
        }
    }
}