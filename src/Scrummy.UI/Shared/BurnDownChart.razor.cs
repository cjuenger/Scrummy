using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Scrummy.Scrum.Contracts.Metrics;
using Scrummy.Scrum.Contracts.Models;
using Scrummy.UI.Models;

namespace Scrummy.UI.Shared
{
    public partial class BurnDownChart
    {
        private List<Xy<DateTime, int>> _opened;
        
        private List<Xy<DateTime, int>> _burnDown;

        private List<Xy<DateTime, int>> _estimate;
        
        private readonly bool _smooth = false;
        
        [Inject]
        private IVelocityCalculator VelocityCalculator { get; set; }
        
        [Parameter] 
        public IEnumerable<Story> Stories { get; set; }

        [Parameter]
        public DateTime StartDate { get; set; }
        
        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            _opened = GetOpenedStoryGraph().ToList();
            _burnDown = GetBurnDownGraph().ToList();
            _estimate = GetEstimationGraph().ToList();
        }
        
        private IEnumerable<Xy<DateTime, int>> GetOpenedStoryGraph()
        {
            var graph = Stories?
                .OrderBy(s => s.CreatedAt)
                .Aggregate(
                    new List<Xy<DateTime, int>>(),
                    (xys, s) =>
                    {
                        var previousCoordinate = xys.LastOrDefault();

                        var xy = new Xy<DateTime, int>
                        {
                            X = s.CreatedAt,
                            Y = (s.StoryPoints ?? 0) + (previousCoordinate?.Y ?? 0)
                        };
                        
                        xys.Add(xy);
                        
                        return xys;
                    }) ?? new List<Xy<DateTime, int>>();
            
            if(!graph.Any()) return Enumerable.Empty<Xy<DateTime, int>>();
            
            graph.Add(new Xy<DateTime, int> {X = DateTime.Now, Y = graph.Last().Y});

            return graph;
        }

        private IEnumerable<Xy<DateTime, int>> GetBurnDownGraph()
        {
            var opened = Stories?
                .OrderBy(s => s.CreatedAt)
                .Select(s => new Xy<DateTime, int>
                {
                    X = s.CreatedAt,
                    Y = s.StoryPoints ?? 0
                }) ?? Enumerable.Empty<Xy<DateTime, int>>();
            
            var closed = GetClosedStoryGraph();

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
            
            var lastBurn = burnDown.LastOrDefault();
            var current = new Xy<DateTime, int>
            {
                X = DateTime.Now,
                Y = lastBurn?.Y ?? 0
            };
            burnDown = burnDown.Append(current).ToList();

            return burnDown;
        }
        
        private IEnumerable<Xy<DateTime, int>> GetClosedStoryGraph()
        {
            return Stories?
                .Where(s => s.ClosedAt != null)
                .OrderBy(s => s.ClosedAt)
                .Select(s => new Xy<DateTime, int>()
                {
                    X = s.ClosedAt.Value,
                    Y = -(s.StoryPoints ?? 0)
                }) ?? Enumerable.Empty<Xy<DateTime, int>>();
        }

        private IEnumerable<Xy<DateTime, int>> GetEstimationGraph()
        {
            var lastBurnDown = _burnDown.LastOrDefault();
            var lastOpened = _opened.LastOrDefault();
            
            if(lastBurnDown == null || lastOpened == null) return Enumerable.Empty<Xy<DateTime, int>>();

            var remainingStoryPoints = lastOpened.Y - lastBurnDown.Y;
            
            var velocity = VelocityCalculator.GetVelocityPerDay(Stories, StartDate, DateTime.Now);

            var daysToGo = remainingStoryPoints / velocity;

            var estimatedXy = new Xy<DateTime, int>
            {
                X = (DateTime) lastBurnDown?.X.AddDays(daysToGo),
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