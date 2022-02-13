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
using Scrummy.DataAccess.GitLab.Parsers;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.DataAccess.GitLab.Providers
{
    internal class SprintProvider : ISprintProvider
    {
        private readonly IProjectApiProvider _projectApiProvider;
        private readonly IItemParser _itemParser;
        private readonly IPaginationService _paginationService;
        private readonly ISprintProviderConfig _config;
        private readonly ILogger<SprintProvider> _logger;

        public SprintProvider(
            IProjectApiProvider projectApiProvider, 
            IItemParser itemParser,
            IPaginationService paginationService,
            ISprintProviderConfig config,
            ILogger<SprintProvider> logger)
        {
            _projectApiProvider = projectApiProvider ?? throw new ArgumentNullException(nameof(projectApiProvider));
            _itemParser = itemParser ?? throw new ArgumentNullException(nameof(itemParser));
            _paginationService = paginationService ?? throw new ArgumentNullException(nameof(paginationService));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<(bool IsSuccess, Sprint Sprint)> TryGetCurrentSprintAsync(string projectId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(projectId));
            }
            
            var sprints = await GetSprintsAsync(projectId, ct);

            var currentSprint = sprints
                .FirstOrDefault(s => 
                    s.StartTime <= DateTime.Now 
                    && s.EndTime >= DateTime.Now);

            if (currentSprint == null) return (false, null);

            currentSprint.Items = (await GetItemsOfSprintAsync(projectId, currentSprint.Name, ct)).ToList();
            
            return (true, currentSprint);
        }
        
        public async Task<IEnumerable<Sprint>> GetAllSprintsAsync(string projectId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(projectId));
            }
            
            var sprints = (await GetSprintsAsync(projectId, ct)).ToArray();
            
            foreach (var sprint in sprints)
            {
                sprint.Items = (await GetItemsOfSprintAsync(projectId, sprint.Name, ct)).ToList();
                
                _logger.LogDebug("Retrieved sprint {SprintName}, Start: {SprintStart}, End: {SprintEnd}, Story Points: {StoryPoints}", 
                    sprint.Name,
                    sprint.StartTime, 
                    sprint.EndTime,
                    sprint.CompletedStoryPoints);
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
            
            var sprints = await GetSprintsAsync(projectId, ct);

            var sprintsInRange = sprints
                .Where(s => 
                    s.StartTime >= startTime 
                    && s.EndTime <= endTime)
                .ToArray();

            foreach (var sprint in sprintsInRange)
            {
                sprint.Items = (await GetItemsOfSprintAsync(projectId, sprint.Name, ct)).ToList();
            }

            return sprintsInRange;
        }
        
        private async Task<IEnumerable<Sprint>> GetSprintsAsync(string projectId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(projectId));
            }

            var sprintLabels = await GetAllSprintLabelsAsync(projectId, ct);

            var sprints = new List<Sprint>();
            
            foreach (var sprintLabel in sprintLabels)
            {
                var (from, to) = GetSprintTimeFromLabelDescription(sprintLabel);
                
                var sprint = new Sprint
                {
                    Name = sprintLabel.Name,
                    StartTime = from,
                    EndTime = to
                };

                sprints.Add(sprint);
            }

            return sprints;
        }

        private async Task<IEnumerable<Label>> GetAllSprintLabelsAsync(string projectId, CancellationToken ct = default)
        {
            var totalLabels = await _paginationService
                .BrowseAllAsync(page => 
                    _projectApiProvider
                        .ProjectApi
                        .GetProjectLabelsAsync(projectId, page, cancellationToken: ct))
                .ConfigureAwait(false);

            return totalLabels.Where(FilterSprintLabel);
        }
        
        private bool FilterSprintLabel(Label label)
        {
            var rgx = new Regex(_config.SprintLabelPattern);
            var match = rgx.Match(label.Name);
            return match.Success;
        }

        private (DateTime From, DateTime To) GetSprintTimeFromLabelDescription(Label label)
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
        
        private async Task<IEnumerable<Item>> GetItemsOfSprintAsync(
            string projectId, 
            string sprintId, 
            CancellationToken ct = default)
        {
            var totalIssues = await _paginationService
                .BrowseAllAsync(page => 
                    _projectApiProvider
                        .ProjectApi
                        .GetProjectIssuesAsync(projectId, page, labels: new List<string> {sprintId}, cancellationToken: ct))
                .ConfigureAwait(false);
            
            var items = totalIssues.Select(i => _itemParser.Parse(i));

            return items;
        }
    }
}