using Microsoft.AspNetCore.Mvc;
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
    private readonly IChartProvider _chartProvider;

    public MetricsController(
        ILogger<MetricsController> logger,
        IVelocityProvider velocityProvider,
        IThroughputProvider throughputProvider,
        IChartProvider chartProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _velocityProvider = velocityProvider ?? throw new ArgumentNullException(nameof(velocityProvider));
        _throughputProvider = throughputProvider ?? throw new ArgumentNullException(nameof(throughputProvider));
        _chartProvider = chartProvider ?? throw new ArgumentNullException(nameof(chartProvider));
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

    [HttpGet("velocityChartData")]
    public Task<VelocityChartData> GetVelocityChartDataAsync(string projectId)
    {
        return _chartProvider.GetVelocityChartData(projectId);
    }

    [HttpGet("throughput")]
    public Task<Throughput> GetThroughputAsync(string projectId)
    {
        return _throughputProvider.GetThroughputTimeAsync(projectId);
    }

    [HttpGet("projectBurnUpChartData")]
    public Task<BurnUpChartData> GetProjectBurnUpChartData(string projectId)
    {
        return _chartProvider.GetProjectBurnUpChartDataAsync(projectId);
    }
    
    [HttpGet("sprintBurnUpChartData")]
    public Task<BurnUpChartData> GetSprintBurnUpChartData(string projectId, int sprintId)
    {
        return _chartProvider.GetSprintBurnUpChartDataAsync(projectId, sprintId);
    }
    
    [HttpGet("releaseBurnUpChartData")]
    public Task<BurnUpChartData> GetReleaseBurnUpChartData(string projectId, int releaseId)
    {
        return _chartProvider.GetReleaseBurnUpChartDataAsync(projectId, releaseId);
    }

    [HttpGet("projectBurnDownChartData")]
    public Task<BurnDownChartData> GetProjectBurnDownChartData(string projectId)
    {
        return _chartProvider.GetProjectBurnDownChartDataAsync(projectId);
    }
    
    [HttpGet("sprintBurnDownChartData")]
    public Task<BurnDownChartData> GetSprintBurnDownChartData(string projectId, int sprintId)
    {
        return _chartProvider.GetSprintBurnDownChartDataAsync(projectId, sprintId);
    }
    
    [HttpGet("releaseBurnDownChartData")]
    public Task<BurnDownChartData> GetReleaseBurnDownChartData(string projectId, int releaseId)
    {
        return _chartProvider.GetReleaseBurnDownChartDataAsync(projectId, releaseId);
    }
}