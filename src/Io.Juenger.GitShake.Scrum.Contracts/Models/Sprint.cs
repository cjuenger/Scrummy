using System;
using System.Collections.Generic;
using System.Linq;
using Scrummy.DataAccess.Contracts.Enums;

namespace Scrummy.Scrum.Contracts.Models
{
    public class Sprint
    {
        public SprintInfo Info { get; }

        public IReadOnlyList<Item> Items { get; }

        public int CompletedStoryPoints => GetCompletedStoryPoints();
        
        public int OpenStoryPoints => GetOpenStoryPoints();

        public Sprint(SprintInfo info, IReadOnlyList<Item> items)
        {
            Info = info ?? throw new ArgumentNullException(nameof(info));
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }
        
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
    }
}