using Microsoft.AspNetCore.Mvc;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Models;

namespace Io.Juenger.Scrummy.Service.Controllers;

[ApiController]
[Route("projects/{projectId}/scrum/[controller]")]
public class SprintsController : ControllerBase
{
    private readonly ILogger<SprintsController> _logger;
    private readonly ISprintProvider _sprintProvider;

    public SprintsController(
        ILogger<SprintsController> logger,
        ISprintProvider sprintProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _sprintProvider = sprintProvider ?? throw new ArgumentNullException(nameof(sprintProvider));
    }

    [HttpGet("infos")]
    public Task<IReadOnlyList<SprintInfo>> GetSprintInfosAsync(string projectId)
    {
        _logger.LogDebug("GET infos of all sprints");
        return _sprintProvider.GetSprintInfosAsync(projectId);
    }
    
    [HttpGet("current/info")]
    public async Task<IActionResult> GetCurrentSprintInfoAsync(string projectId)
    {
        _logger.LogDebug("GET infos of current sprint");
        
        var result = await _sprintProvider.TryGetCurrentSprintInfoAsync(projectId);

        if (result.IsSuccess) return Ok(result.SprintInfo);
        
        _logger.LogDebug("NOT FOUND - No current sprint info found");
        return NotFound();
    }
    
    [HttpGet("{sprintId:int}/info")]
    public async Task<IActionResult> GetSprintInfoAsync(string projectId, int sprintId)
    {
        _logger.LogDebug("GET info of sprint of id {SprintId}", sprintId);
        var result = await _sprintProvider.TryGetSprintInfoAsync(projectId, sprintId);

        if (result.IsSuccess) return Ok(result.SprintInfo);
        
        _logger.LogDebug("NOT FOUND - No sprint info of id {SprintId} found", sprintId);
        return NotFound();
    }

    [HttpGet]
    public async Task<IReadOnlyList<Sprint>> GetSprintsAsync(string projectId)
    {
        _logger.LogDebug("GET all sprints");
        
        var sprints = await _sprintProvider.GetAllSprintsAsync(projectId);
        return sprints.ToList();
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentSprintAsync(string projectId)
    {
        _logger.LogDebug("GET current sprint");
        var result = await _sprintProvider.TryGetCurrentSprintAsync(projectId);

        if (result.IsSuccess) return Ok(result.Sprint);
        
        _logger.LogDebug("NOT FOUND - No current sprint found");
        return NotFound();
    }

    [HttpGet("{sprintId:int}")]
    public async Task<IActionResult> GetSprintAsync(string projectId, int sprintId)
    {
        _logger.LogDebug("GET info of sprint of id {SprintId}", sprintId);
        var result = await _sprintProvider.TryGetSprintAsync(projectId, sprintId);

        if (result.IsSuccess) return Ok(result.Sprint);
        
        _logger.LogDebug("NOT FOUND - No sprint of id {SprintId} found", sprintId);
        return NotFound();
    }
}