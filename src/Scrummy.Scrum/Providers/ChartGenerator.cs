using System;
using System.Collections.Generic;
using System.Linq;
using Scrummy.Scrum.Contracts.Models;
using Scrummy.Scrum.Contracts.Providers;

namespace Scrummy.Scrum.Providers
{
    public class ChartGenerator : IChartGenerator
    {
        public IEnumerable<Xy<DateTime, int>> GetOpenedStoryChart(IEnumerable<Story> stories)
        {
            var graph = stories?
                .OrderBy(s => s.CreatedAt)
                .Select(s => new Xy<DateTime, int>
                {
                    X = s.CreatedAt,
                    Y = s.StoryPoints ?? 0
                }) ?? new List<Xy<DateTime, int>>();

            return graph;
        }
        
        public IEnumerable<Xy<DateTime, int>> GetCumulatedOpenedStoryChart(IEnumerable<Story> stories, bool tillToday = true)
        {
            var graph = stories?
                .OrderBy(s => s.CreatedAt)
                .Aggregate(
                    new List<Xy<DateTime, int>>(),
                    (coordinates, s) =>
                    {
                        var previousCoordinate = coordinates.LastOrDefault();

                        var xy = new Xy<DateTime, int>
                        {
                            X = s.CreatedAt,
                            Y = (s.StoryPoints ?? 0) + (previousCoordinate?.Y ?? 0)
                        };
                        
                        coordinates.Add(xy);
                        
                        return coordinates;
                    }) ?? new List<Xy<DateTime, int>>();
            
            if(!graph.Any()) return Enumerable.Empty<Xy<DateTime, int>>();

            if (tillToday)
            {
                graph.Add(new Xy<DateTime, int> {X = DateTime.Now, Y = graph.Last().Y});    
            }
            
            return graph;
        }
        
        public IEnumerable<Xy<DateTime, int>> GetClosedStoryChart(IEnumerable<Story> stories)
        {
            var graph = stories?
                .Where(s => s.ClosedAt != null)
                .OrderBy(s => s.ClosedAt)
                .Select(s => new Xy<DateTime, int>
                {
                    X = s.ClosedAt.Value,
                    Y = -(s.StoryPoints ?? 0)
                }) ?? Enumerable.Empty<Xy<DateTime, int>>();

            return graph;
        }
        
        public IEnumerable<Xy<DateTime, int>> GetCumulatedClosedStoryChart(IEnumerable<Story> stories, bool tillToday = true)
        {
            var graph = stories?
                .Where(s => s.ClosedAt != null)
                .OrderBy(s => s.ClosedAt)
                .Select(s => new Xy<DateTime, int>()
                {
                    X = s.ClosedAt.Value,
                    Y = s.StoryPoints ?? 0
                })
                .ToList() ?? new List<Xy<DateTime, int>>();
            
            if(!graph.Any()) return Enumerable.Empty<Xy<DateTime, int>>();

            if (tillToday)
            {
                graph.Add(new Xy<DateTime, int> {X = DateTime.Now, Y = graph.Last().Y});    
            }

            return graph;
        }
        
        public IEnumerable<Xy<DateTime, int>> GetBurnUpChart(IEnumerable<Story> stories, bool tillToday = true)
        {
            var storyArray = stories?.ToArray() ?? Array.Empty<Story>();
            
            var opened = GetCumulatedOpenedStoryChart(storyArray, false);
            var closed = GetCumulatedClosedStoryChart(storyArray, false);

            var graph = opened
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
            
            if(!graph.Any()) return Enumerable.Empty<Xy<DateTime, int>>();

            if (tillToday)
            {
                graph.Add(new Xy<DateTime, int> {X = DateTime.Now, Y = graph.Last().Y});
            }

            return graph;
        }
        
        public IEnumerable<Xy<DateTime, int>> GetBurnDownChart(IEnumerable<Story> stories, bool tillToday = true)
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
        
        public IEnumerable<Xy<DateTime, int>> GetBurnDownEstimationChart(IEnumerable<Story> stories, double velocity)
        {
            var storyArray = stories?.ToArray() ?? Array.Empty<Story>();
            
            var burnDown = GetBurnDownChart(storyArray);
            var opened = GetCumulatedOpenedStoryChart(storyArray);
            
            var lastBurnDown = burnDown.LastOrDefault();
            var lastOpened = opened.LastOrDefault();
            
            if(lastBurnDown == null || lastOpened == null) return Enumerable.Empty<Xy<DateTime, int>>();

            var remainingStoryPoints = lastOpened.Y - lastBurnDown.Y;

            var daysToGo = remainingStoryPoints / velocity;

            var estimatedXy = new Xy<DateTime, int>
            {
                X = lastBurnDown.X.AddDays(daysToGo),
                Y = 0
            };

            return new[]
            {
                lastBurnDown,
                estimatedXy
            };
        }
       
    }
}