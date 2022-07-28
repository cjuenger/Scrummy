using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Models;
using Scrummy.Scrum.Metrics.Calculators;

namespace Scrummy.Scrum.Metrics
{
    internal class BurnUpChartDataProvider : IBurnUpChartDataProvider
    {
        private readonly IStorySeriesCalculator _storySeriesCalculator;
        private readonly IItemsProvider _itemsProvider;
        private readonly IReleaseProvider _releaseProvider;

        public BurnUpChartDataProvider(
            IStorySeriesCalculator storySeriesCalculator,
            IItemsProvider itemsProvider,
            IReleaseProvider releaseProvider)
        {
            _storySeriesCalculator = storySeriesCalculator ?? throw new ArgumentNullException(nameof(storySeriesCalculator));
            _itemsProvider = itemsProvider ?? throw new ArgumentNullException(nameof(itemsProvider));
            _releaseProvider = releaseProvider ?? throw new ArgumentNullException(nameof(releaseProvider));
        }
        
        public async Task<BurnUpChartData> GetProjectBurnUpChartDataAsync(string projectId, CancellationToken ct = default)
        {
            var items = await _itemsProvider.GetItemsOfProjectAsync(projectId, ct);
            return GetBurnUpChartData(items);
        }
       
        public async Task<BurnUpChartData> GetReleaseBurnUpChartDataAsync(string projectId, int releaseId, CancellationToken ct = default)
        {
            var (isSuccess, releaseInfo) = await _releaseProvider.TryGetReleaseInfoAsync(projectId, releaseId, ct);

            if (!isSuccess) return new BurnUpChartData();
            
            var items = await _itemsProvider.GetItemsOfReleaseAsync(projectId, releaseInfo, ct);

            return GetBurnUpChartData(items);
        }
        
        private BurnUpChartData GetBurnUpChartData(IEnumerable<Item> items, bool tillToday = true)
        {
            var stories = items.OfType<Story>();
            var storyArray = stories.ToArray();
            
            var opened = _storySeriesCalculator.CalculateCumulatedOpenedStoryChart(storyArray, false);
            var closed = _storySeriesCalculator.CalculateCumulatedClosedStoryChart(storyArray, false);

            var burnUpSeries = opened
                .Select(xy => new Xy<DateTime, int>
                {
                    X = xy.X, 
                    Y = 0
                })
                .Concat(closed)
                .OrderBy(xy => xy.X)
                .Aggregate(
                    new List<Xy<DateTime, int>>(),
                    (xys, xy) =>
                    {
                        var previousXy = xys.LastOrDefault();
                        
                        var newXy = new Xy<DateTime, int>
                        {
                            X = xy.X,
                            Y = xy.Y + (previousXy?.Y ?? 0)
                        };
                        
                        xys.Add(newXy);
                        
                        return xys;
                    });
            
            if(!burnUpSeries.Any()) return new BurnUpChartData();

            if (tillToday)
            {
                burnUpSeries.Add(new Xy<DateTime, int> {X = DateTime.Now, Y = burnUpSeries.Last().Y});
            }

            return new BurnUpChartData
            {
                BurnUpSeries = burnUpSeries
            };
        }
    }
}