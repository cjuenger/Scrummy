using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Io.Juenger.Common.Util;
using Microsoft.Extensions.Logging;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Models;
using Scrummy.Scrum.Metrics.Calculators;

namespace Scrummy.Scrum.Metrics
{
    internal class BurnDownChartDataProvider : IBurnDownChartDataProvider
    {
        private readonly ILogger<BurnDownChartDataProvider> _logger;
        private readonly IVelocityProvider _velocityProvider;
        private readonly IItemsProvider _itemsProvider;
        private readonly IReleaseProvider _releaseProvider;
        private readonly IStorySeriesCalculator _storySeriesCalculator;

        public BurnDownChartDataProvider(
            ILogger<BurnDownChartDataProvider> logger,
            IVelocityProvider velocityProvider,
            IItemsProvider itemsProvider,
            IReleaseProvider releaseProvider,
            IStorySeriesCalculator storySeriesCalculator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _velocityProvider = velocityProvider ?? throw new ArgumentNullException(nameof(velocityProvider));
            _itemsProvider = itemsProvider ?? throw new ArgumentNullException(nameof(itemsProvider));
            _releaseProvider = releaseProvider ?? throw new ArgumentNullException(nameof(releaseProvider));
            _storySeriesCalculator = storySeriesCalculator ?? throw new ArgumentNullException(nameof(storySeriesCalculator));
        }
        
        public async Task<BurnDownChartData> GetProjectBurnDownChartDataAsync(string projectId, CancellationToken ct = default)
        {
            var items = await _itemsProvider.GetItemsOfProjectAsync(projectId, ct);
            return await GetBurnDownChartDataAsync(projectId, items, ct);
        }
        
        public async Task<BurnDownChartData> GetReleaseBurnDownChartDataAsync(string projectId, int releaseId, CancellationToken ct = default)
        {
            var (isSuccess, releaseInfo) = await _releaseProvider.TryGetReleaseInfoAsync(projectId, releaseId, ct);

            if (!isSuccess) return new BurnDownChartData();
            
            var items = await _itemsProvider.GetItemsOfReleaseAsync(projectId, releaseInfo, ct);

            return await GetBurnDownChartDataAsync(projectId, items, ct);
        }
        
        private IEnumerable<Xy<DateTime, int>> GetBurnDownChartSeries(IEnumerable<Story> stories, bool tillToday = true)
        {
            var storyArray = stories?.ToArray() ?? Array.Empty<Story>();
            
            var opened = storyArray.OrderBy(s => s.CreatedAt)
                .Select(s => new Xy<DateTime, int>
                {
                    X = s.CreatedAt,
                    Y = s.StoryPoints ?? 0
                });
            
            var closed = _storySeriesCalculator.CalculateClosedStoryChart(storyArray);

            var burnDown = opened
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

            if (burnDown.Count <= 0) return burnDown;

            if (!tillToday) return burnDown;
            
            var lastBurn = burnDown.LastOrDefault();
            var current = new Xy<DateTime, int>
            {
                X = DateTime.Now,
                Y = lastBurn?.Y ?? 0
            };
            burnDown = burnDown.Append(current).ToList();

            return burnDown;
        }

        private IEnumerable<Xy<DateTime, int>> GetBurnDownEstimationChartSeries(IEnumerable<Story> stories, float velocityPerDay)
        {
            var storyArray = stories?.ToArray() ?? Array.Empty<Story>();
            
            var burnDown = GetBurnDownChartSeries(storyArray);
            
            var lastBurnDown = burnDown.LastOrDefault();
            
            if(lastBurnDown == null) return Enumerable.Empty<Xy<DateTime, int>>();

            var remainingStoryPoints = lastBurnDown.Y;
            
            // NOTE, if the velocity is zero a velocity of 0.1 (equals 1 SP per a two weeks sprint)
            // story point per day is assumed.
            velocityPerDay = velocityPerDay <= 0 ? 0.1f : velocityPerDay;

            var daysToGo = remainingStoryPoints / velocityPerDay;

            _logger.LogDebug(
                "{DaysToGo} days to go to complete {StoryPoints} story points by a velocity of {VelocityPerDay} per day", 
                daysToGo, 
                remainingStoryPoints,
                velocityPerDay);

            // TODO: 20220212 CJ: Consider hours of a work day!
            var dueDate = lastBurnDown.X.GetBusinessDueDate(daysToGo);
            
            var estimatedXy = new Xy<DateTime, int>
            {
                X = dueDate,
                Y = 0
            };

            return new[]
            {
                lastBurnDown,
                estimatedXy
            };
        }

        private async Task<BurnDownChartData> GetBurnDownChartDataAsync(
            string projectId, 
            IEnumerable<Item> items, 
            CancellationToken ct)
        {
            var stories = items.OfType<Story>().ToList();
            
            var burnDownSeries = GetBurnDownChartSeries(stories);
            
            var velocity = await _velocityProvider.GetVelocityAsync(projectId, ct);
            
            var estimateSeries = GetBurnDownEstimationChartSeries(
                stories, 
                velocity.DayAverageVelocity);
            
            var bestEstimateSeries = GetBurnDownEstimationChartSeries(
                stories, 
                velocity.Best3SprintsDayAverageVelocity);
            
            var worstEstimateSeries = GetBurnDownEstimationChartSeries(
                stories, 
                velocity.Worst3SprintsDayAverageVelocity);

            return new BurnDownChartData
            {
                BurnDownSeries = burnDownSeries,
                EstimateSeries = estimateSeries,
                BestEstimateSeries = bestEstimateSeries,
                WorstEstimateSeries = worstEstimateSeries
            };
        }
    }
}