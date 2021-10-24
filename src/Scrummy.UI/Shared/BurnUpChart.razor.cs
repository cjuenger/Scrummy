using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Scrummy.Scrum.Contracts.Models;
using Scrummy.UI.Models;

namespace Scrummy.UI.Shared
{
    public partial class BurnUpChart
    {
        private List<Xy<DateTime, int>> _opened;
        
        private List<Xy<DateTime, int>> _burnUp;
        
        private readonly bool _smooth = false;
        
        [Parameter] 
        public IEnumerable<Story> Stories { get; set; }
        
        [Parameter]
        public DateTime StartDate { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            _opened = GetOpenedStoryGraph()?.ToList();
            _burnUp = GetBurnUpGraph()?.ToList();
        }

        private IEnumerable<Xy<DateTime, int>> GetOpenedStoryGraph()
        {
            var graph = Stories?
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
            
            graph.Add(new Xy<DateTime, int> {X = DateTime.Now, Y = graph.Last().Y});

            return graph;
        }

        private IEnumerable<Xy<DateTime, int>> GetBurnUpGraph()
        {
            var opened = GetOpenedStoryGraph();
            var closed = GetClosedStoryGraph();

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
            
            graph.Add(new Xy<DateTime, int> {X = DateTime.Now, Y = graph.Last().Y});

            return graph;
        }

        private IEnumerable<Xy<DateTime, int>> GetClosedStoryGraph()
        {
            return Stories?
                .Where(s => s.ClosedAt != null)
                .OrderBy(s => s.ClosedAt)
                .Select(s => new Xy<DateTime, int>()
                {
                    X = s.ClosedAt.Value,
                    Y = s.StoryPoints ?? 0
                }) ?? Enumerable.Empty<Xy<DateTime, int>>();
        }
    }
}