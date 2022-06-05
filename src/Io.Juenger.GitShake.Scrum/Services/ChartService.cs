using System;
using System.Collections.Generic;
using System.Linq;
using Io.Juenger.Common.Util;
using Microsoft.Extensions.Logging;
using Scrummy.Scrum.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.Scrum.Services
{
    internal class ChartService : IChartService
    {
        private readonly ILogger<ChartService> _logger;

        public ChartService(ILogger<ChartService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
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
        
        public IEnumerable<Xy<DateTime, int>> GetBurnDownEstimationChart(IEnumerable<Story> stories, float velocityPerDay)
        {
            var storyArray = stories?.ToArray() ?? Array.Empty<Story>();
            
            var burnDown = GetBurnDownChart(storyArray);
            
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

        public IReadOnlyList<DataSeries<string, int>> GetVelocityChart(IEnumerable<Sprint> sprints)
        {
            var sprintArray = sprints?.ToArray() ?? Array.Empty<Sprint>();

            var sprintsAsStoryPoints = sprintArray
                .OrderBy(s => s.Info.EndTime)
                .Aggregate(new List<Xy<string, int>>(), (aggregate, sprint) =>
                {
                    var averageXy = new Xy<string, int>
                    {
                        X = $"Sprint {aggregate.Count+1}",
                        Y = sprint.CompletedStoryPoints
                    };
                    
                    aggregate.Add(averageXy);
                    
                    return aggregate;
                });

            if (sprintsAsStoryPoints.Count <= 0) return Enumerable.Empty<DataSeries<string, int>>().ToList();

            var accumulatedStoryPoints = sprintsAsStoryPoints
                .Aggregate(new List<int>(), (aggregate, sprint) =>
                {
                    var accumulatedValue = aggregate.LastOrDefault() + sprint.Y;
                    aggregate.Add(accumulatedValue);

                    return aggregate;
                });

            // Simple Moving Average
            var smaVelocity = accumulatedStoryPoints
                .Aggregate(new List<Xy<string, int>>(), (aggregate, accSp) =>
                {
                    var average = accSp/(aggregate.Count+1);

                    var averageXy = new Xy<string, int>
                    {
                        X = $"Sprint {aggregate.Count+1}",
                        Y = average
                    };
                    
                    aggregate.Add(averageXy);
                    
                    return aggregate;
                });

            var dataSeries = new List<DataSeries<string, int>>
            {
                new() { Series = sprintsAsStoryPoints, Title = "Completed Stories" },
                new() { Series = smaVelocity, Title = "SMA Velocity" }
            };
        
            return dataSeries;
        }
    }
}