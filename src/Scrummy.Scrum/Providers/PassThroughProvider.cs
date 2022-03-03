using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Scrummy.DataAccess.Contracts.Enums;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Enums;
using Scrummy.Scrum.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.Scrum.Providers
{
    public class PassThroughProvider : IPassThroughProvider
    {
        private readonly IItemsProvider _itemsProvider;

        public PassThroughProvider(IItemsProvider itemsProvider)
        {
            _itemsProvider = itemsProvider ?? throw new ArgumentNullException(nameof(itemsProvider));
        }
        
        public async Task<PassThrough> GetPassThroughTimeAsync(string projectId, CancellationToken ct = default)
        {
            var items = (await _itemsProvider.GetAllItemsAsync(projectId, ct)).ToList();

            var stories = items.Where(i => i.Type == ItemType.Story).ToList();
            var bugs = items.Where(i => i.Type == ItemType.Bug).ToList();
            var others = items.Where(i => i.Type == ItemType.Other).ToList();

            var storyPassThroughTimeAsTicks = GetPassThroughTimesAsTicks(stories.Where(i => i.ClosedAt.HasValue));
            var bugPassThroughTimeAsTicks = GetPassThroughTimesAsTicks(bugs.Where(i => i.ClosedAt.HasValue));
            var otherPassThroughTimeAsTicks = GetPassThroughTimesAsTicks(others.Where(i => i.ClosedAt.HasValue));
            
            var passThroughTime = new PassThrough
            {
                AverageStoryPassThroughTime = GetAveragePassThroughTime(storyPassThroughTimeAsTicks),
                WorstStoryPassThroughTime = GetMaxPassThroughTime(storyPassThroughTimeAsTicks),
                BestStoryPassThroughTime = GetMinPassThroughTime(storyPassThroughTimeAsTicks),
                
                AverageBugPassThroughTime = GetAveragePassThroughTime(bugPassThroughTimeAsTicks),
                WorstBugPassThroughTime = GetMaxPassThroughTime(bugPassThroughTimeAsTicks),
                BestBugPassThroughTime = GetMinPassThroughTime(bugPassThroughTimeAsTicks),
                
                AverageOtherPassThroughTime = GetAveragePassThroughTime(otherPassThroughTimeAsTicks),
                WorstOtherPassThroughTime = GetMaxPassThroughTime(otherPassThroughTimeAsTicks),
                BestOtherPassThroughTime = GetMinPassThroughTime(otherPassThroughTimeAsTicks)
            };

            return passThroughTime;
        }

        private static ICollection<long> GetPassThroughTimesAsTicks(IEnumerable<Item> items)
        {
            var passThroughTimesAsTicks = items
                .Select(i => (i.ClosedAt.Value - i.CreatedAt).Ticks);

            return passThroughTimesAsTicks.ToList();
        }

        private static TimeSpan GetAveragePassThroughTime(ICollection<long> ticks)
        {
            var totalPassThroughTimeAsTicks = ticks.Sum();
            var averagePassThroughTimeAsTicks = totalPassThroughTimeAsTicks / ticks.Count;
            return TimeSpan.FromTicks(averagePassThroughTimeAsTicks);
        }

        private static TimeSpan GetMaxPassThroughTime(IEnumerable<long> ticks)
        {
            var passThroughTimesAsTicks = ticks.Max();
            return TimeSpan.FromTicks(passThroughTimesAsTicks);
        }
        
        private static TimeSpan GetMinPassThroughTime(IEnumerable<long> ticks)
        {
            var passThroughTimesAsTicks = ticks.Min();
            return TimeSpan.FromTicks(passThroughTimesAsTicks);
        }
    }
}