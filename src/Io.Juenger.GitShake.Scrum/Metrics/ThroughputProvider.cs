using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Enums;
using Scrummy.Scrum.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.Scrum.Metrics
{
    internal class ThroughputProvider : IThroughputProvider
    {
        private readonly IItemsProvider _itemsProvider;

        public ThroughputProvider(IItemsProvider itemsProvider)
        {
            _itemsProvider = itemsProvider ?? throw new ArgumentNullException(nameof(itemsProvider));
        }
        
        public async Task<Throughput> GetThroughputTimeAsync(string projectId, CancellationToken ct = default)
        {
            var items = (await _itemsProvider.GetItemsOfProjectAsync(projectId, ct)).ToList();

            var stories = items.Where(i => i.Type == ItemType.Story).ToList();
            var bugs = items.Where(i => i.Type == ItemType.Bug).ToList();
            var others = items.Where(i => i.Type == ItemType.Other).ToList();

            var storyThroughputTimeAsTicks = GetThroughputTimesAsTicks(stories.Where(i => i.ClosedAt.HasValue));
            var bugThroughputTimeAsTicks = GetThroughputTimesAsTicks(bugs.Where(i => i.ClosedAt.HasValue));
            var otherThroughputTimeAsTicks = GetThroughputTimesAsTicks(others.Where(i => i.ClosedAt.HasValue));
            
            var throughputTime = new Throughput
            {
                AverageStoryThroughputTime = GetAverageThroughputTime(storyThroughputTimeAsTicks),
                WorstStoryThroughputTime = GetMaxThroughputTime(storyThroughputTimeAsTicks),
                BestStoryThroughputTime = GetMinThroughputTime(storyThroughputTimeAsTicks),
                
                AverageBugThroughputTime = GetAverageThroughputTime(bugThroughputTimeAsTicks),
                WorstBugThroughputTime = GetMaxThroughputTime(bugThroughputTimeAsTicks),
                BestBugThroughputTime = GetMinThroughputTime(bugThroughputTimeAsTicks),
                
                AverageOtherThroughputTime = GetAverageThroughputTime(otherThroughputTimeAsTicks),
                WorstOtherThroughputTime = GetMaxThroughputTime(otherThroughputTimeAsTicks),
                BestOtherThroughputTime = GetMinThroughputTime(otherThroughputTimeAsTicks)
            };

            return throughputTime;
        }

        private static ICollection<long> GetThroughputTimesAsTicks(IEnumerable<Item> items)
        {
            var throughputTimesAsTicks = items
                .Select(i => (i.ClosedAt!.Value - i.CreatedAt).Ticks);

            return throughputTimesAsTicks.ToList();
        }

        private static TimeSpan GetAverageThroughputTime(ICollection<long> ticks)
        {
            var totalThroughputTimeAsTicks = ticks.Sum();
            var averageThroughputTimeAsTicks = totalThroughputTimeAsTicks / ticks.Count;
            return TimeSpan.FromTicks(averageThroughputTimeAsTicks);
        }

        private static TimeSpan GetMaxThroughputTime(IEnumerable<long> ticks)
        {
            var throughputTimesAsTicks = ticks.Max();
            return TimeSpan.FromTicks(throughputTimesAsTicks);
        }
        
        private static TimeSpan GetMinThroughputTime(IEnumerable<long> ticks)
        {
            var throughputTimesAsTicks = ticks.Min();
            return TimeSpan.FromTicks(throughputTimesAsTicks);
        }
    }
}