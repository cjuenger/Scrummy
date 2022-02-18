using System;
using System.Collections.Generic;
using System.Linq;
using Scrummy.DataAccess.Contracts.Enums;
using Scrummy.Scrum.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.Scrum.Services
{
    internal class VelocityCalculator : IVelocityCalculator
    {
        public Velocity CalculateVelocity(IEnumerable<Sprint> sprints)
        {
            var velocity = new Velocity();
            
            var sprintsWithStoryPoints = sprints
                .Where(sp => sp.CompletedStoryPoints > 0)
                .ToList();
            velocity.CountOfSprints = sprintsWithStoryPoints.Count;

            CalculateAverageSprintLength(sprintsWithStoryPoints, ref velocity);
            CalculateTotalTimeRange(sprintsWithStoryPoints, ref velocity);
            CalculateVelocity(sprintsWithStoryPoints, ref velocity);
            
            return velocity;
        }
        
        private static void CalculateTotalTimeRange(IEnumerable<Sprint> sprints, ref Velocity velocity)
        {
            var timeOrderedSprints = sprints
                .OrderBy(sp => sp.Info.StartTime)
                .ToList();

            velocity.Start = timeOrderedSprints.FirstOrDefault()?.Info.StartTime ?? default;
            velocity.End = timeOrderedSprints.LastOrDefault()?.Info.EndTime ?? default;
        }
        
        private static void CalculateVelocity(IEnumerable<Sprint> sprints, ref Velocity velocity)
        {
            var storyPoints = sprints
                .Select(sp => 
                    sp.Items
                        .OfType<Story>()
                        .Where(st => st.State == WorkflowState.Closed)
                        .Sum(st => st.StoryPoints ?? 0))
                .OrderBy(sp => sp)
                .ToList();
            
            velocity.AverageVelocity = GetSprintAverageVelocity(storyPoints);
            velocity.Best3SprintsAverageVelocity = GetSprintAverageVelocity(storyPoints.TakeLast(3).ToList());
            velocity.Worst3SprintsAverageVelocity = GetSprintAverageVelocity(storyPoints.Take(3).ToList());

            CalculateDayAverageVelocity(ref velocity);
        }
        
        private static void CalculateDayAverageVelocity(ref Velocity velocity)
        {
            velocity.DayAverageVelocity = velocity.AverageVelocity / velocity.AverageSprintLength;
            velocity.Best3SprintsDayAverageVelocity = velocity.Best3SprintsAverageVelocity / velocity.AverageSprintLength;
            velocity.Worst3SprintsDayAverageVelocity = velocity.Worst3SprintsAverageVelocity / velocity.AverageSprintLength;
        }
        
        private static float GetSprintAverageVelocity(IReadOnlyCollection<int> storyPoints)
        {
            var totalStoryPoints = storyPoints.Sum();
            var averageStoryPoints = (float) totalStoryPoints / Math.Max(1,storyPoints.Count);
            return averageStoryPoints;
        }

        private static void CalculateAverageSprintLength(IReadOnlyCollection<Sprint> sprints, ref Velocity velocity)
        {
            var totalLengthOfSprints = sprints.Sum(sp => sp.Info.Length);
            var averageSprintLength = (float) totalLengthOfSprints / sprints.Count;

            velocity.AverageSprintLength = averageSprintLength;
        }
    }
}