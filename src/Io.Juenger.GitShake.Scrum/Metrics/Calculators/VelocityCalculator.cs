using System;
using System.Collections.Generic;
using System.Linq;
using Scrummy.DataAccess.Contracts.Enums;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.Scrum.Metrics.Calculators
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
        
        public VelocityChartData CalculateVelocityChartData(IEnumerable<Sprint> sprints)
        {
            var sprintList = sprints.ToList();
            var velocity = CalculateVelocity(sprintList);
            var velocityChartData = new VelocityChartData {Velocity = velocity};

            var sprintsAsStoryPoints = sprintList
                .OrderBy(s => s.Info.EndTime)
                .Aggregate(new List<Xy<string, int>>(), (aggregate, sprint) =>
                {
                    var averageXy = new Xy<string, int>
                    {
                        X = $"Sprint {aggregate.Count+1}",
                        Y = sprint.CompletedStoryPoints
                    };
                    
                    aggregate.Add(averageXy);
                    
                    return aggregate;
                });

            if (sprintsAsStoryPoints.Count <= 0) return velocityChartData;

            var accumulatedStoryPoints = sprintsAsStoryPoints
                .Aggregate(new List<int>(), (aggregate, sprint) =>
                {
                    var accumulatedValue = aggregate.LastOrDefault() + sprint.Y;
                    aggregate.Add(accumulatedValue);

                    return aggregate;
                });

            // Simple Moving Average
            var smaVelocity = accumulatedStoryPoints
                .Aggregate(new List<Xy<string, int>>(), (aggregate, accSp) =>
                {
                    var average = accSp/(aggregate.Count+1);

                    var averageXy = new Xy<string, int>
                    {
                        X = $"Sprint {aggregate.Count+1}",
                        Y = average
                    };
                    
                    aggregate.Add(averageXy);
                    
                    return aggregate;
                });

            var dataSeries = new List<DataSeries<string, int>>
            {
                new() { Series = sprintsAsStoryPoints, Title = "Completed Stories" },
                new() { Series = smaVelocity, Title = "SMA Velocity" }
            };

            velocityChartData.VelocitySeries = dataSeries;
        
            return velocityChartData;
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