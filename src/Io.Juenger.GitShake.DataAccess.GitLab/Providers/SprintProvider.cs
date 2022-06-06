using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Io.Juenger.GitLabClient.Model;
using Microsoft.Extensions.Logging;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.DataAccess.GitLab.Configs;
using Scrummy.DataAccess.GitLab.Services;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.DataAccess.GitLab.Providers
{
    internal class SprintProvider : ISprintProvider
    {
        private readonly ISprintProviderConfig _config;
        private readonly ILogger<SprintProvider> _logger;
        private readonly IPaginationService _paginationService;
        private readonly IProjectApiProvider _projectApiProvider;
        private readonly IItemsProvider _itemsProvider;

        public SprintProvider(
            IProjectApiProvider projectApiProvider, 
            IItemsProvider itemsProvider,
            IPaginationService paginationService,
            ISprintProviderConfig config,
            ILogger<SprintProvider> logger)
        {
            _projectApiProvider = projectApiProvider ?? throw new ArgumentNullException(nameof(projectApiProvider));
            _itemsProvider = itemsProvider ?? throw new ArgumentNullException(nameof(itemsProvider));
            _paginationService = paginationService ?? throw new ArgumentNullException(nameof(paginationService));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IReadOnlyList<SprintInfo>> GetSprintInfosAsync(string projectId, CancellationToken ct = default)
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

        public async Task<(bool IsSuccess, SprintInfo SprintInfo)> TryGetCurrentSprintInfoAsync(
            string projectId, 
            CancellationToken ct = default)
        {
            var sprintInfos = 
                await GetSprintInfosAsync(projectId, ct)
                .ConfigureAwait(false);

            var now = DateTime.UtcNow;
            var currentSprintInfo =
                sprintInfos.FirstOrDefault(sprintInfo => now >= sprintInfo.StartTime && now <= sprintInfo.EndTime);

            return currentSprintInfo == null ? (false, null) : (true, currentSprintInfo);
        }

        public async Task<(bool IsSuccess, SprintInfo SprintInfo)> TryGetSprintInfoAsync(
            string projectId, 
            int sprintId, 
            CancellationToken ct = default)
        {
            var sprintInfos = 
                await GetSprintInfosAsync(projectId, ct)
                .ConfigureAwait(false);

            var currentSprintInfo =
                sprintInfos.FirstOrDefault(sprintInfo => sprintInfo.Id == sprintId);

            return currentSprintInfo == null ? (false, null) : (true, currentSprintInfo);
        }

        public async Task<Sprint> GetSprintAsync(
            string projectId, 
            SprintInfo sprintInfo,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(projectId));
            }
            
            var items = 
                await _itemsProvider
                    .GetItemsOfSprintAsync(projectId, sprintInfo.Name, ct)
                    .ConfigureAwait(false);
            
            var sprint = new Sprint(sprintInfo, items);
            
            _logger.LogDebug(
                "Retrieved sprint {SprintName}, Start: {SprintStart}, End: {SprintEnd}, Story Points: {StoryPoints}", 
                sprint.Info.Name,
                sprint.Info.StartTime, 
                sprint.Info.EndTime,
                sprint.CompletedStoryPoints);

            return sprint;
        }

        public async Task<(bool IsSuccess, Sprint Sprint)> TryGetCurrentSprintAsync(
            string projectId, 
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(projectId));
            }
            
            var sprintInfos = 
                await GetSprintInfosAsync(projectId, ct).
                ConfigureAwait(false);

            var currentSprintInfo = sprintInfos
                .FirstOrDefault(sprintInfo => 
                    sprintInfo.StartTime <= DateTime.Now 
                    && sprintInfo.EndTime >= DateTime.Now);

            if (currentSprintInfo == null) return (false, null);

            var currentSprint = await GetSprintAsync(projectId, currentSprintInfo, ct)
                .ConfigureAwait(false);
            
            return (true, currentSprint);
        }

        public async Task<(bool IsSuccess, Sprint Sprint)> TryGetSprintAsync(
            string projectId, 
            int sprintId, 
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(projectId));
            }

            var sprintInfo = await TryGetSprintInfoAsync(projectId, sprintId, ct);
            if (!sprintInfo.IsSuccess) return (false, null);

            var sprint = await GetSprintAsync(projectId, sprintInfo.SprintInfo, ct)
                .ConfigureAwait(false);
            
            return (true, sprint);
        }

        public async Task<IEnumerable<Sprint>> GetAllSprintsAsync(string projectId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(projectId));
            }

            var sprintInfos = 
                await GetSprintInfosAsync(projectId, ct).
                ConfigureAwait(false);

            var sprints = new List<Sprint>();
            
            foreach (var sprintInfo in sprintInfos)
            {
                var sprint = await GetSprintAsync(projectId, sprintInfo, ct).ConfigureAwait(false);
                sprints.Add(sprint);
            }

            return sprints;
        }

        public async Task<IEnumerable<Sprint>> GetSprintsInRange(
            string projectId, 
            DateTime startTime, 
            DateTime endTime, 
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(projectId));
            }

            var sprintInfos = 
                await GetSprintInfosAsync(projectId, ct).
                ConfigureAwait(false);
            
            var sprintInfosInRange = sprintInfos
                .Where(sprintInfo => 
                    sprintInfo.StartTime >= startTime 
                    && sprintInfo.EndTime <= endTime)
                .ToArray();

            var sprints = await GetSprintsAsync(projectId, sprintInfosInRange, ct)
                .ConfigureAwait(false);

            return sprints;
        }

        public async Task<IEnumerable<Sprint>> GetSprintsAsync(
            string projectId,
            IEnumerable<SprintInfo> sprintInfos,
            CancellationToken ct = default)
        {
            var sprints = new List<Sprint>();
            
            foreach (var sprintInfo in sprintInfos)
            {
                var sprint = await GetSprintAsync(projectId, sprintInfo, ct).ConfigureAwait(false);
                sprints.Add(sprint);
            }

            return sprints;
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