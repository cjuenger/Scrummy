using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Enums;
using Scrummy.Scrum.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.Scrum.Metrics
{
    internal class SprintMetrics : ISprintMetrics
    {
        private readonly IItemsProvider _itemsProvider;
        private readonly ISprintProvider _sprintProvider;
        private readonly IBurnDownChartDataProvider _burnDownChartDataProvider;
        private readonly IBurnUpChartDataProvider _burnUpChartDataProvider;

        public SprintMetrics(
            IItemsProvider itemsProvider, 
            ISprintProvider sprintProvider,
            IBurnDownChartDataProvider burnDownChartDataProvider,
            IBurnUpChartDataProvider burnUpChartDataProvider)
        {
            _itemsProvider = itemsProvider ?? throw new ArgumentNullException(nameof(itemsProvider));
            _sprintProvider = sprintProvider ?? throw new ArgumentNullException(nameof(sprintProvider));
            _burnDownChartDataProvider = burnDownChartDataProvider ?? throw new ArgumentNullException(nameof(burnDownChartDataProvider));
            _burnUpChartDataProvider = burnUpChartDataProvider ?? throw new ArgumentNullException(nameof(burnUpChartDataProvider));
        }
        
        public async Task<SprintComposition> GetSprintCompositionAsync(
            string projectId, 
            int sprintId, 
            CancellationToken ct = default)
        {
            var (isSuccess, sprintInfo) = await _sprintProvider.TryGetSprintInfoAsync(projectId, sprintId, ct);

            if (!isSuccess) return new SprintComposition();
            
            var items = await _itemsProvider.GetItemsOfSprintAsync(projectId, sprintInfo, ct);
            
            var countOfTotalItems = items.Count;
            var countOfStories = items.OfType<Story>().Count();
            var countOfBugs = items.Count(i => i.Type == ItemType.Bug);
            var countOfOthers = countOfTotalItems - countOfStories - countOfBugs;

            return new SprintComposition
            {
                SprintInfo = sprintInfo,
                CountOfStories = countOfStories,
                CountOfBugs = countOfBugs,
                CountOfOthers = countOfOthers
            };
        }
        
        public async Task<BurnDownChartData> GetSprintBurnDownChartDataAsync(string projectId, int sprintId, CancellationToken ct = default)
        {
            var (isSuccess, sprintInfo) = await _sprintProvider.TryGetSprintInfoAsync(projectId, sprintId, ct);

            if (!isSuccess) return new BurnDownChartData();
            
            var items = await _itemsProvider.GetItemsOfSprintAsync(projectId, sprintInfo, ct);

            return await _burnDownChartDataProvider.GetBurnDownChartDataAsync(projectId, items, ct);
        }
        
        public async Task<BurnUpChartData> GetSprintBurnUpChartDataAsync(string projectId, int sprintId, CancellationToken ct = default)
        {
            var (isSuccess, sprintInfo) = await _sprintProvider.TryGetSprintInfoAsync(projectId, sprintId, ct);

            if (!isSuccess) return new BurnUpChartData();
            
            var items = await _itemsProvider.GetItemsOfSprintAsync(projectId, sprintInfo, ct);

            return _burnUpChartDataProvider.GetBurnUpChartData(items);
        }
    }
}