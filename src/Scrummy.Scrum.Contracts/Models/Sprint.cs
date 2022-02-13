using System;
using System.Collections.Generic;
using System.Linq;
using Io.Juenger.Common.Util;
using Scrummy.DataAccess.Contracts.Enums;

namespace Scrummy.Scrum.Contracts.Models
{
    public class Sprint
    {
        public string Name { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int Length => GetSprintLength();
        
        public List<Item> Items { get; set; }

        public int CompletedStoryPoints => GetCompletedStoryPoints();

        public int OpenStoryPoints => GetOpenStoryPoints();
        
        private int GetCompletedStoryPoints()
        {
            if (!Items.Any()) return 0;

            var velocity = Items
                .OfType<Story>()
                .Where(s => s.State == WorkflowState.Closed)
                .Sum(s => s.StoryPoints ?? 0);
            
            return velocity;
        }

        private int GetOpenStoryPoints()
        {
            if (!Items.Any()) return 0;

            var velocity = Items
                .OfType<Story>()
                .Where(s => s.State != WorkflowState.Closed)
                .Sum(s => s.StoryPoints ?? 0);
            
            return velocity;
        }

        private int GetSprintLength()
        {
            var businessDays = StartTime.GetBusinessDaysUntil(EndTime);
            return businessDays;
        }
    }
}