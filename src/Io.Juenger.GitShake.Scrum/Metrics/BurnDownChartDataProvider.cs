using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Io.Juenger.Common.Util;
using Microsoft.Extensions.Logging;
using Scrummy.Scrum.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.Scrum.Metrics
{
    internal class BurnDownChartDataProvider : IBurnDownChartDataProvider
    {
        private readonly ILogger<BurnDownChartDataProvider> _logger;
        private readonly IVelocityProvider _velocityProvider;

        public BurnDownChartDataProvider(
            ILogger<BurnDownChartDataProvider> logger,
            IVelocityProvider velocityProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _velocityProvider = velocityProvider ?? throw new ArgumentNullException(nameof(velocityProvider));
        }
        
        public IEnumerable<Xy<DateTime, int>> GetBurnDownChartSeries(IEnumerable<Story> stories, bool tillToday = true)
        {
            var storyArray = stories?.ToArray() ?? Array.Empty<Story>();
            
            var opened = storyArray.OrderBy(s => s.CreatedAt)
                .Select(s => new Xy<DateTime, int>
                {
                    X = s.CreatedAt,
                    Y = s.StoryPoints ?? 0
                });
            
            var closed = GetClosedStoryChart(storyArray);

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

        public IEnumerable<Xy<DateTime, int>> GetBurnDownEstimationChartSeries(IEnumerable<Story> stories, float velocityPerDay)
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

        public async Task<BurnDownChartData> GetBurnDownChartDataAsync(
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