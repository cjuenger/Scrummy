using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using IO.Juenger.GitLab.Api;
using IO.Juenger.GitLab.Model;
using Scrummy.DataAccess.Contracts.Exceptions;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.DataAccess.Contracts.Models;
using Scrummy.DataAccess.GitLab.Configs;
using Scrummy.DataAccess.GitLab.Parsers;

namespace Scrummy.DataAccess.GitLab.Providers
{
    internal class SprintProvider : ISprintProvider
    {
        private readonly IProjectApiProvider _projectApiProvider;
        private readonly IItemParser _itemParser;
        private readonly ISprintProviderConfig _config;
        
        public SprintProvider(
            IProjectApiProvider projectApiProvider, 
            IItemParser itemParser,
            ISprintProviderConfig config)
        {
            _projectApiProvider = projectApiProvider ?? throw new ArgumentNullException(nameof(projectApiProvider));
            _itemParser = itemParser ?? throw new ArgumentNullException(nameof(itemParser));
            _config = config ?? throw new ArgumentNullException(nameof(config));
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
            
            var page = 1;
            List<Label> pagedLabels;
            var totalLabels = new List<Label>();
            do
            {
                pagedLabels = await _projectApiProvider.ProjectApi
                    .GetProjectLabelsAsync(projectId, cancellationToken: ct)
                    .ConfigureAwait(false);
                
                totalLabels.AddRange(pagedLabels);
                page++;
            } 
            while (pagedLabels.Any());

            var sprintLabels = totalLabels.Where(FilterSprintLabel);

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
            var page = 1;
            List<Issue> pagedIssues;
            var totalIssues = new List<Issue>();
            do
            {
                pagedIssues = await _projectApiProvider.ProjectApi
                    .GetProjectIssuesAsync(projectId, page, labels: new List<string> {sprintId}, cancellationToken: ct)
                    .ConfigureAwait(false);
                
                totalIssues.AddRange(pagedIssues);
                page++;
            } 
            while (pagedIssues.Any());
            
            var items = totalIssues.Select(i => _itemParser.Parse(i));

            return items;
        }
    }
}