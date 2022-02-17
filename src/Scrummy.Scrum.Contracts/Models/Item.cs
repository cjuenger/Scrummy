using System;
using System.Collections.Generic;
using Scrummy.DataAccess.Contracts.Enums;
using Scrummy.Scrum.Contracts.Enums;

namespace Scrummy.Scrum.Contracts.Models
{
    public class Item
    {
        public string Title { get; set; }
        
        public ItemType Type { get; set; }

        public WorkflowState State { get; set; }
        
        public string Description { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime StartedAt { get; set; }

        public DateTime? ClosedAt { get; set; }
        
        public IEnumerable<string> Tasks { get; set; }

        public string Link { get; set; }
    }
}