using System;
using System.Collections.Generic;
using System.Linq;
using Scrummy.DataAccess.Contracts.Enums;

namespace Scrummy.DataAccess.Contracts.Models
{
    public class Sprint
    {
        public string Name { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public List<Item> Items { get; set; }

        public int StoryPoints => GetStoryPoints();

        private int GetStoryPoints()
        {

            if (!Items.Any()) return 0;

            var velocity = Items
                .OfType<Story>()
                .Where(s => s.State == WorkflowState.Closed)
                .Sum(s => s.StoryPoints ?? 0);
            
            return velocity;
        }
    }
}