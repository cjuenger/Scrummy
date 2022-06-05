using Microsoft.AspNetCore.Mvc;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Models;

namespace Io.Juenger.Scrummy.Service.Controllers;

[ApiController]
[Route("projects/{projectId}/scrum/[controller]")]
public class BacklogController : ControllerBase
{
    private readonly ILogger<BacklogController> _logger;
    private readonly IItemsProvider _itemsProvider;

    public BacklogController(
        ILogger<BacklogController> logger,
        IItemsProvider itemsProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _itemsProvider = itemsProvider ?? throw new ArgumentNullException(nameof(itemsProvider));
    }
    
    [HttpGet("items")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<Item>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllItems(string projectId)
    {
        _logger.LogDebug("GET all backlog items");
        var items = await _itemsProvider.GetItemsOfProjectAsync(projectId);
        return Ok(items);
    }
}