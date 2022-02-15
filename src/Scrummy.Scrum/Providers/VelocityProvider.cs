using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Scrummy.DataAccess.Contracts.Enums;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.Scrum.Providers
{
    internal class VelocityProvider : IVelocityProvider
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

        public async Task CalculateVelocityAsync(string projectId, DateTime endTime, CancellationToken ct = default)
        {
            if (projectId == null) throw new ArgumentNullException(nameof(projectId));
            
            var sprints= await _sprintProvider
                .GetAllSprintsAsync(projectId, ct)
                .ConfigureAwait(false);

            var sprintsWithStoryPoints = sprints
                .Where(sp => sp.EndTime <= endTime)
                .Where(sp => sp.CompletedStoryPoints > 0)
                .ToList();
            
            CalculateTotalTimeRange(sprintsWithStoryPoints);
            CalculateVelocity(sprintsWithStoryPoints);
            CalculateDayAverageVelocity(sprintsWithStoryPoints);
        }

        public Task CalculateVelocityAsync(string projectId, CancellationToken ct = default)
        {
            return CalculateVelocityAsync(projectId, DateTime.UtcNow, ct);
        }

        private void CalculateTotalTimeRange(IEnumerable<Sprint> sprints)
        {
            var timeOrderedSprints = sprints
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

            SprintAverageVelocity = GetSprintAverageVelocity(storyPoints);
            Best3SprintsAverageVelocity = GetSprintAverageVelocity(storyPoints.TakeLast(3).ToList());
            Worst3SprintsAverageVelocity = GetSprintAverageVelocity(storyPoints.Take(3).ToList());
        }

        private void CalculateDayAverageVelocity(IReadOnlyCollection<Sprint> sprints)
        {
            var totalLengthOfSprints = sprints.Sum(sp => sp.Length);
            var averageSprintLength = (float) totalLengthOfSprints / sprints.Count;

            DayAverageVelocity = SprintAverageVelocity / averageSprintLength;
            Best3SprintsDayAverageVelocity = Best3SprintsAverageVelocity / averageSprintLength;
            Worst3SprintsDayAverageVelocity = Worst3SprintsAverageVelocity / averageSprintLength;
        }
        
        private static float GetSprintAverageVelocity(IReadOnlyCollection<int> storyPoints)
        {
            var totalStoryPoints = storyPoints.Sum();
            var averageStoryPoints = (float) totalStoryPoints / Math.Max(1,storyPoints.Count);
            return averageStoryPoints;
        }
    }
}