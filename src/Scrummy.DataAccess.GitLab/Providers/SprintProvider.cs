using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.DataAccess.GitLab.Parsers;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.DataAccess.GitLab.Providers
{
    internal class SprintProvider : ISprintProvider
    {
        private readonly IProjectApiProvider _projectApiProvider;
        private readonly IItemParser _itemParser;
        private readonly IPaginationService _paginationService;
        private readonly ISprintInfoProvider _sprintInfoProvider;
        private readonly ILogger<SprintProvider> _logger;

        public SprintProvider(
            IProjectApiProvider projectApiProvider, 
            IItemParser itemParser,
            IPaginationService paginationService,
            ISprintInfoProvider sprintInfoProvider,
            ILogger<SprintProvider> logger)
        {
            _projectApiProvider = projectApiProvider ?? throw new ArgumentNullException(nameof(projectApiProvider));
            _itemParser = itemParser ?? throw new ArgumentNullException(nameof(itemParser));
            _paginationService = paginationService ?? throw new ArgumentNullException(nameof(paginationService));
            _sprintInfoProvider = sprintInfoProvider ?? throw new ArgumentNullException(nameof(sprintInfoProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            
            var items = await GetItemsOfSprintAsync(projectId, sprintInfo.Name, ct)
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

        public async Task<IEnumerable<Sprint>> GetSprintsAsync(string projectId,
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
        
        public async Task<(bool IsSuccess, Sprint Sprint)> TryGetCurrentSprintAsync(
            string projectId, 
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(projectId));
            }
            
            var sprintInfos = await _sprintInfoProvider.
                GetAllSprintsAsync(projectId, ct).
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
        
        public async Task<IEnumerable<Sprint>> GetAllSprintsAsync(string projectId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(projectId));
            }

            var sprintInfos = await _sprintInfoProvider.
                GetAllSprintsAsync(projectId, ct).
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

            var sprintInfos = await _sprintInfoProvider.
                GetAllSprintsAsync(projectId, ct).
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
        
        private async Task<IReadOnlyList<Item>> GetItemsOfSprintAsync(
            string projectId, 
            string sprintName, 
            CancellationToken ct = default)
        {
            var totalIssues = await _paginationService
                .BrowseAllAsync(page => 
                    _projectApiProvider
                        .ProjectApi
                        .GetProjectIssuesAsync(projectId, page, labels: new List<string> {sprintName}, cancellationToken: ct))
                .ConfigureAwait(false);
            
            var items = totalIssues.Select(i => _itemParser.Parse(i));

            return items.ToList();
        }
    }
}