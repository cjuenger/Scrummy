using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using IO.Juenger.GitLab.Api;
using IO.Juenger.GitLab.Model;
using Microsoft.Extensions.Configuration;
using Scrummy.DataAccess.Contracts.Exceptions;
using Scrummy.DataAccess.Contracts.Models;
using Scrummy.DataAccess.Contracts.Providers;
using Scrummy.DataAccess.GitLab.Parsers;

namespace Scrummy.DataAccess.GitLab.Providers
{
    internal class SprintProvider : ISprintProvider
    {
        private const string DefaultTimePattern = @"(From:|To:)\s*(\d{4})-(\d{2})-(\d{2})(T(\d{2}):(\d{2})(:(\d{2}))?)?";
        private const string DefaultSprintLabelPattern = "(Sprint|sprint)";
        
        private readonly IProjectApi _projectApi;
        private readonly IItemParser _itemParser;

        private readonly string _sprintTimePattern;
        private readonly string _sprintLabelPattern;
        
        public SprintProvider(
            IProjectApi projectApi, 
            IItemParser itemParser,
            IConfiguration configuration)
        {
            _projectApi = projectApi ?? throw new ArgumentNullException(nameof(projectApi));
            _itemParser = itemParser ?? throw new ArgumentNullException(nameof(itemParser));

            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var section = configuration.GetSection("GitLab");

            _sprintTimePattern = section.GetValue("SprintTimePattern",DefaultTimePattern);
            _sprintLabelPattern = section.GetValue("SprintLabelPattern", DefaultSprintLabelPattern);
        }
        
        public async Task<Sprint> GetCurrentSprintAsync(string projectId, CancellationToken ct = default)
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

            if (currentSprint == null) throw new SprintNotFoundException();
            
            currentSprint.Items = (await GetItemsOfSprintAsync(projectId, currentSprint.Name, ct)).ToList();
            
            return currentSprint;
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
            
            var labels = await _projectApi
                .GetProjectLabelsAsync(projectId, cancellationToken: ct)
                .ConfigureAwait(false);

            var sprintLabels = labels.Where(FilterSprintLabel);

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
            var rgx = new Regex(_sprintLabelPattern);
            var match = rgx.Match(label.Name);
            return match.Success;
        }

        private (DateTime From, DateTime To) GetSprintTimeFromLabelDescription(Label label)
        {
            var rgx = new Regex(_sprintTimePattern);
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
            var issues = await _projectApi
                .GetProjectIssuesAsync(projectId, labels: new List<string> {sprintId}, cancellationToken: ct)
                .ConfigureAwait(false);
            
            var items = issues.Select(i => _itemParser.Parse(i));

            return items;
        }
    }
}