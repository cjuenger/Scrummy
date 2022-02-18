using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Scrummy.DataAccess.Contracts.Enums;
using Scrummy.Scrum.Contracts.Models;
using Scrummy.Scrum.Services;
using ArrangeContext.NSubstitute;
using FluentAssertions;

namespace Scrummy.Scrum.Tests
{
    [TestFixture]
    public class VelocityCalculatorTests
    {
        [Test]
        public void CalculateVelocity()
        {
            var ctx = new ArrangeContext<VelocityCalculator>();
            var sut = ctx.Build();

            const int weeksPerSprint = 2;
            const int businessWeeksPerWeek = 5;

            var sprints = TestHelper.GetDummySprints();

            var velocity = sut.CalculateVelocity(sprints);

            Assert(velocity, sprints, weeksPerSprint, businessWeeksPerWeek);
        }

        [Test]
        public void CalculateVelocity_With_Some_Sprints_Without_Story_Points()
        {
            var ctx = new ArrangeContext<VelocityCalculator>();
            var sut = ctx.Build();

            const int weeksPerSprint = 2;
            const int businessWeeksPerWeek = 5;

            var sprints = TestHelper.GetDummySprints().ToList();

            var sprint3 = sprints[2];
            sprints.Remove(sprint3);
            sprint3 = new Sprint(sprint3.Info, Enumerable.Empty<Item>().ToList());
            sprints.Insert(2, sprint3);
            
            var sprint5 = sprints[4];
            sprints.Remove(sprint5);
            sprint5 = new Sprint(sprint5.Info, Enumerable.Empty<Item>().ToList());
            sprints.Insert(2, sprint5);
            
            var velocity = sut.CalculateVelocity(sprints);
            
            sprints.Remove(sprint3);
            sprints.Remove(sprint5);
            
            Assert(velocity, sprints, weeksPerSprint, businessWeeksPerWeek);
        }

        private static void Assert(
            Velocity velocity, 
            IReadOnlyList<Sprint> sprints, 
            int weeksPerSprint, 
            int businessWeeksPerWeek)
        {
            var orderedSprintStoryPoints = sprints
                .Select(sp => sp.Items
                    .OfType<Story>()
                    .Where(st => st.State == WorkflowState.Closed)
                    .Select(st => st.StoryPoints ?? 0)
                    .Sum())
                .OrderBy(stp => stp)
                .ToList();
            
            var totalClosedStoryPoints = orderedSprintStoryPoints.Sum();
            var expectedSprintAverageVelocity = (float) totalClosedStoryPoints / sprints.Count;
            var expectedDayAverageVelocity = (float) totalClosedStoryPoints / (sprints.Count * weeksPerSprint * businessWeeksPerWeek);

            var listOfClosedBest3StoryPoints = orderedSprintStoryPoints.TakeLast(3).ToList();
            var closedStoryPointsOfBestThreeSprints = listOfClosedBest3StoryPoints.Sum();
            var expectedBest3SprintsAverageVelocity = (float) closedStoryPointsOfBestThreeSprints / listOfClosedBest3StoryPoints.Count;
            var expectedBest3SprintsDayAverageVelocity = expectedBest3SprintsAverageVelocity / (weeksPerSprint * businessWeeksPerWeek);
            
            var listOfClosedWorst3StoryPoints = orderedSprintStoryPoints.Take(3).ToList();
            var closedStoryPointsOfWorstThreeSprints = listOfClosedWorst3StoryPoints.Sum();
            var expectedWorst3SprintsAverageVelocity = (float) closedStoryPointsOfWorstThreeSprints / listOfClosedWorst3StoryPoints.Count;
            var expectedWorst3SprintsDayAverageVelocity = expectedWorst3SprintsAverageVelocity / (weeksPerSprint * businessWeeksPerWeek);
            
            velocity.AverageVelocity.Should().Be(expectedSprintAverageVelocity);
            velocity.DayAverageVelocity.Should().Be(expectedDayAverageVelocity);
            velocity.Best3SprintsAverageVelocity.Should().Be(expectedBest3SprintsAverageVelocity);
            velocity.Best3SprintsDayAverageVelocity.Should().Be(expectedBest3SprintsDayAverageVelocity);
            velocity.Worst3SprintsAverageVelocity.Should().Be(expectedWorst3SprintsAverageVelocity);
            velocity.Worst3SprintsDayAverageVelocity.Should().Be(expectedWorst3SprintsDayAverageVelocity);
            velocity.Start.Should().Be(sprints[0].Info.StartTime);
            velocity.End.Should().Be(sprints[^1].Info.EndTime);
        }
    }
}