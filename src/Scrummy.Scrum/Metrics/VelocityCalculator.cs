using System;
using System.Collections.Generic;
using System.Linq;
using Io.Juenger.Common.Util;
using Scrummy.DataAccess.Contracts.Enums;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.Scrum.Metrics
{
    public class VelocityCalculator : IVelocityCalculator
    {
        public int GetAverageVelocityPerSprintWeeks(IEnumerable<Story> stories, DateTime start, DateTime end, int sprintWeeks)
        {
            var velocityPerDay = GetAverageVelocityPerSingleDay(stories, start, end);
            var velocity = (int) (sprintWeeks * 5 * velocityPerDay);
            return velocity;
        }
        
        public double GetAverageVelocityPerSingleDay(IEnumerable<Story> stories, DateTime start, DateTime end)
        {
            if (stories == null) throw new ArgumentNullException(nameof(stories));

            var storiesList = stories.ToList();
            var closedStories = storiesList
                .Where(s => s.State == WorkflowState.Closed)
                .Sum(s => s.StoryPoints ?? 0);
            
            var businessDays = start.GetBusinessDaysUntil(end);

            if (businessDays <= 0) businessDays = 1; // TODO: 20210813 CJ: Should I calculate this this way?
            
            return (double) closedStories / businessDays;
        }

        public double GetAverageVelocityPerDaySinceStart(IEnumerable<Story> stories, DateTime start)
        {
            if (stories == null) throw new ArgumentNullException(nameof(stories));

            var storiesList = stories.ToList();
            var closedStories = storiesList
                .Where(s => s.State == WorkflowState.Closed)
                .Sum(s => s.StoryPoints ?? 0);

            var end = storiesList
                .OrderBy(s => s.ClosedAt ?? DateTime.MinValue )
                .LastOrDefault(s => s.State == WorkflowState.Closed)
                ?.ClosedAt ?? start;
            
            var businessDays = start.GetBusinessDaysUntil(end);

            if (businessDays <= 0) businessDays = 1; // TODO: 20210813 CJ: Should I calculate this this way?
            
            return (double) closedStories / businessDays;
        }
    }
}