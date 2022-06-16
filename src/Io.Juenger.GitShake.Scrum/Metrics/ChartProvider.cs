using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.Scrum.Metrics
{
    internal class ChartProvider : IChartProvider
    {
        private readonly IItemsProvider _itemsProvider;
        private readonly ILogger<ChartProvider> _logger;
        private readonly IReleaseProvider _releaseProvider;
        private readonly IBurnDownChartDataProvider _burnDownChartDataProvider;
        private readonly IBurnUpChartDataProvider _burnUpChartDataProvider;
        private readonly ISprintProvider _sprintProvider;
        private readonly IVelocityProvider _velocityProvider;

        public ChartProvider(
            ILogger<ChartProvider> logger,
            IVelocityProvider velocityProvider,
            IItemsProvider itemsProvider,
            ISprintProvider sprintProvider,
            IReleaseProvider releaseProvider,
            IBurnDownChartDataProvider burnDownChartDataProvider,
            IBurnUpChartDataProvider burnUpChartDataProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _velocityProvider = velocityProvider ?? throw new ArgumentNullException(nameof(velocityProvider));
            _itemsProvider = itemsProvider ?? throw new ArgumentNullException(nameof(itemsProvider));
            _sprintProvider = sprintProvider ?? throw new ArgumentNullException(nameof(sprintProvider));
            _releaseProvider = releaseProvider ?? throw new ArgumentNullException(nameof(releaseProvider));
            _burnDownChartDataProvider = burnDownChartDataProvider ?? throw new ArgumentNullException(nameof(burnDownChartDataProvider));
            _burnUpChartDataProvider = burnUpChartDataProvider ?? throw new ArgumentNullException(nameof(burnUpChartDataProvider));
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

        public async Task<BurnDownChartData> GetProjectBurnDownChartDataAsync(string projectId, CancellationToken ct = default)
        {
            var items = await _itemsProvider.GetItemsOfProjectAsync(projectId, ct);
            return await _burnDownChartDataProvider.GetBurnDownChartDataAsync(projectId, items, ct);
        }
        
        public async Task<BurnDownChartData> GetReleaseBurnDownChartDataAsync(string projectId, int releaseId, CancellationToken ct = default)
        {
            var (isSuccess, releaseInfo) = await _releaseProvider.TryGetReleaseInfoAsync(projectId, releaseId, ct);

            if (!isSuccess) return new BurnDownChartData();
            
            var items = await _itemsProvider.GetItemsOfReleaseAsync(projectId, releaseInfo, ct);

            return await _burnDownChartDataProvider.GetBurnDownChartDataAsync(projectId, items, ct);
        }

        public async Task<BurnUpChartData> GetProjectBurnUpChartDataAsync(string projectId, CancellationToken ct = default)
        {
            var items = await _itemsProvider.GetItemsOfProjectAsync(projectId, ct);
            return _burnUpChartDataProvider.GetBurnUpChartData(items);
        }
       
        public async Task<BurnUpChartData> GetReleaseBurnUpChartDataAsync(string projectId, int releaseId, CancellationToken ct = default)
        {
            var (isSuccess, releaseInfo) = await _releaseProvider.TryGetReleaseInfoAsync(projectId, releaseId, ct);

            if (!isSuccess) return new BurnUpChartData();
            
            var items = await _itemsProvider.GetItemsOfReleaseAsync(projectId, releaseInfo, ct);

            return _burnUpChartDataProvider.GetBurnUpChartData(items);
        }
        
        public async Task<VelocityChartData> GetVelocityChartData(string projectId, CancellationToken ct = default)
        {
            var velocity = await _velocityProvider.GetVelocityAsync(projectId, ct);
            var velocityChartData = new VelocityChartData {Velocity = velocity};
            
            var sprints = await _sprintProvider.GetAllSprintsAsync(projectId, ct);
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

            if (sprintsAsStoryPoints.Count <= 0) return velocityChartData;

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

            velocityChartData.VelocitySeries = dataSeries;
        
            return velocityChartData;
        }

        
    }
}