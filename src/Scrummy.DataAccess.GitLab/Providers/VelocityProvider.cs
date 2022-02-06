using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Scrummy.DataAccess.Contracts.Enums;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.DataAccess.Contracts.Models;
using Scrummy.DataAccess.GitLab.Util;

namespace Scrummy.DataAccess.GitLab.Providers
{
    public class VelocityProvider : IVelocityProvider
    {
        private readonly ISprintProvider _sprintProvider;
        public float SprintAverageVelocity { get; private set; }
        public float DayAverageVelocity { get; private set; }
        public float Best3SprintsAverageVelocity { get; private set; }
        
        public float Best3SprintsDayAverageVelocity { get; private set; }
        public float Worst3SprintsAverageVelocity { get; private set; }
        
        public float Worst3SprintsDayAverageVelocity { get; private set; }

        public DateTime StartTimeOfFirstSprint { get; set; }

        public DateTime EndTimeOfLastSprint { get; set; }
        
        public VelocityProvider(ISprintProvider sprintProvider)
        {
            _sprintProvider = sprintProvider ?? throw new ArgumentNullException(nameof(sprintProvider));
        }

        public async Task LoadVelocityAsync(string projectId, CancellationToken ct = default)
        {
            if (projectId == null) throw new ArgumentNullException(nameof(projectId));
            
            var sprints= await _sprintProvider
                .GetAllSprintsAsync(projectId, ct)
                .ConfigureAwait(false);

            var listOfSprints = sprints.ToList();
            
            CalculateTotalTimeRange(listOfSprints);
            CalculateVelocity(listOfSprints);
            CalculateDayAverageVelocity();
        }

        private void CalculateTotalTimeRange(IEnumerable<Sprint> sprints)
        {
            var now = DateTime.UtcNow;
            var timeOrderedSprints = sprints
                .Where(sp => sp.EndTime < now)
                .OrderBy(sp => sp.StartTime)
                .ToList();

            StartTimeOfFirstSprint = timeOrderedSprints.FirstOrDefault()?.StartTime ?? default;
            EndTimeOfLastSprint = timeOrderedSprints.LastOrDefault()?.EndTime ?? default;
        }

        private void CalculateVelocity(IEnumerable<Sprint> sprints)
        {
            var storyPoints = sprints
                .Select(sp => 
                    sp.Items
                        .OfType<Story>()
                        .Where(st => st.State == WorkflowState.Closed)
                        .Sum(st => st.StoryPoints ?? 0))
                .OrderBy(sp => sp)
                .ToList();

            SprintAverageVelocity = GetVelocity(storyPoints);
            Best3SprintsAverageVelocity = GetVelocity(storyPoints.TakeLast(3).ToList());
            Worst3SprintsAverageVelocity = GetVelocity(storyPoints.Take(3).ToList());
        }

        private void CalculateDayAverageVelocity()
        {
            var businessDays = StartTimeOfFirstSprint.GetBusinessDaysUntil(EndTimeOfLastSprint);

            DayAverageVelocity = SprintAverageVelocity / Math.Max(1, businessDays);
            Best3SprintsDayAverageVelocity = Best3SprintsAverageVelocity / businessDays;
            Worst3SprintsDayAverageVelocity = Worst3SprintsAverageVelocity / businessDays;

        }
        
        private static float GetVelocity(IReadOnlyCollection<int> storyPoints)
        {
            var totalStoryPoints = storyPoints.Sum();
            var averageStoryPoints = totalStoryPoints / Math.Max(1,storyPoints.Count);
            return averageStoryPoints;
        }
    }
}