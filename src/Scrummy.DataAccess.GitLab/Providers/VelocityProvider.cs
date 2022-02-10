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

            var sprintsWithStoryPoints = sprints
                .Where(sp => sp.StoryPoints > 0)
                .ToList();
            
            CalculateTotalTimeRange(sprintsWithStoryPoints);
            CalculateVelocity(sprintsWithStoryPoints);
            CalculateDayAverageVelocity(sprintsWithStoryPoints.Count);
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

            SprintAverageVelocity = GetSprintAverageVelocity(storyPoints);
            Best3SprintsAverageVelocity = GetSprintAverageVelocity(storyPoints.TakeLast(3).ToList());
            Worst3SprintsAverageVelocity = GetSprintAverageVelocity(storyPoints.Take(3).ToList());
        }

        private void CalculateDayAverageVelocity(int countOfSprints)
        {
            var businessDays = StartTimeOfFirstSprint.GetBusinessDaysUntil(EndTimeOfLastSprint);
            var averageBusinessDaysPerSprint = (float) businessDays / countOfSprints;

            DayAverageVelocity = SprintAverageVelocity / averageBusinessDaysPerSprint;
            Best3SprintsDayAverageVelocity = Best3SprintsAverageVelocity / averageBusinessDaysPerSprint;
            Worst3SprintsDayAverageVelocity = Worst3SprintsAverageVelocity / averageBusinessDaysPerSprint;

        }
        
        private static float GetSprintAverageVelocity(IReadOnlyCollection<int> storyPoints)
        {
            var totalStoryPoints = storyPoints.Sum();
            var averageStoryPoints = totalStoryPoints / Math.Max(1,storyPoints.Count);
            return averageStoryPoints;
        }
    }
}