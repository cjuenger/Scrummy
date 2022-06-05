using Microsoft.AspNetCore.Mvc;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Models;

namespace Io.Juenger.Scrummy.Service.Controllers;

[ApiController]
[Route("projects/{projectId}/scrum/[controller]")]
public class MetricsController : ControllerBase
{
    private readonly ILogger<MetricsController> _logger;
    private readonly IVelocityProvider _velocityProvider;
    private readonly IThroughputProvider _throughputProvider;
    private readonly IChartService _chartService;
    private readonly IItemsProvider _itemsProvider;

    public MetricsController(
        ILogger<MetricsController> logger,
        IVelocityProvider velocityProvider,
        IThroughputProvider throughputProvider,
        IChartService chartService,
        IItemsProvider itemsProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _velocityProvider = velocityProvider ?? throw new ArgumentNullException(nameof(velocityProvider));
        _throughputProvider = throughputProvider ?? throw new ArgumentNullException(nameof(throughputProvider));
        _chartService = chartService ?? throw new ArgumentNullException(nameof(chartService));
        _itemsProvider = itemsProvider ?? throw new ArgumentNullException(nameof(itemsProvider));
    }

    [HttpGet("velocity")]
    public Task<Velocity> GetVelocityAsync(string projectId, DateTime? from = null, DateTime? to = null)
    {
        _logger.LogDebug(
            "GET Velocity from project {ProjectId} from {From} to {To}", 
            projectId,
            from,
            to);

        return to.HasValue ? 
            _velocityProvider.GetVelocityAsync(projectId, to.Value) : 
            _velocityProvider.GetVelocityAsync(projectId);
    }

    [HttpGet("throughput")]
    public Task<Throughput> GetThroughputAsync(string projectId)
    {
        return _throughputProvider.GetThroughputTimeAsync(projectId);
    }

    [HttpGet("burnUp")]
    public async Task<IEnumerable<Xy<DateTime, int>>> GetBurnUp(string projectId)
    {
        // TODO: 20220603 CJ: Request only issues with 'Story' label!
        var items = await _itemsProvider.GetItemsOfProjectAsync(projectId);
        var stories = items.OfType<Story>();
        return _chartService.GetBurnUpChart(stories);
    }

    [HttpGet("burnDown")]
    public async Task<IEnumerable<Xy<DateTime, int>>> GetBurnDown(string projectId)
    {
        // TODO: 20220603 CJ: Request only issues with 'Story' label!
        var items = await _itemsProvider.GetItemsOfProjectAsync(projectId);
        var stories = items.OfType<Story>();
        return _chartService.GetBurnDownChart(stories);
    }
}