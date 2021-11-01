using System;
using System.Collections.Generic;
using System.Linq;
using Scrummy.DataAccess.Contracts.Enums;
using Scrummy.DataAccess.Contracts.Models;
using Scrummy.Scrum.Util;

namespace Scrummy.Scrum.Metrics
{
    public class VelocityCalculator : IVelocityCalculator
    {
        public int GetVelocityPerSprint(IEnumerable<Story> stories, DateTime start, DateTime end, int sprintWeeks)
        {
            var velocityPerDay = GetVelocityPerDay(stories, start, end);
            var velocity = (int) (sprintWeeks * 5 * velocityPerDay);
            return velocity;
        }
        
        public double GetVelocityPerDay(IEnumerable<Story> stories, DateTime start, DateTime end)
        {
            if (stories == null) throw new ArgumentNullException(nameof(stories));

            var storiesList = stories.ToList();
            var storyPoints = storiesList.Sum(s => s.StoryPoints ?? 0);
            var closedStories = storiesList.Where(s => s.State == WorkflowState.Closed);
            
            // var lastClosedTime = closedStories
            //     .OrderBy(s => s.ClosedAt ?? DateTime.MinValue)
            //     .Select(s => s.ClosedAt)
            //     .LastOrDefault();
            //
            // if (lastClosedTime == null) return 0;
            
            var businessDays = start.GetBusinessDaysUntil(end);

            if (businessDays <= 0) businessDays = 1; // TODO: 20210813 CJ: Should I calculate this this way?
            
            return (double) storyPoints / businessDays;
        }
    }
}