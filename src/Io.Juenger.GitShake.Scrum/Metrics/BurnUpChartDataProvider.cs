using System;
using System.Collections.Generic;
using System.Linq;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.Scrum.Metrics
{
    internal class BurnUpChartDataProvider : IBurnUpChartDataProvider
    {
        public BurnUpChartData GetBurnUpChartData(IEnumerable<Item> items, bool tillToday = true)
        {
            var stories = items.OfType<Story>();
            var storyArray = stories.ToArray();
            
            var opened = GetCumulatedOpenedStoryChart(storyArray, false);
            var closed = GetCumulatedClosedStoryChart(storyArray, false);

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