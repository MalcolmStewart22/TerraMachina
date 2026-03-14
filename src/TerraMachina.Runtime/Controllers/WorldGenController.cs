using Microsoft.AspNetCore.Mvc;
using TerraMachina.Runtime.Services;

namespace TerraMachina.Runtime.Controllers;

[ApiController]
[Route("worldgen")]
public class WorldGenController : ControllerBase
{
    private readonly IEngineService _engineService;

    public WorldGenController(IEngineService engineService)
    {
        _engineService = engineService;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartAsync(StartWorldGenRequest request)
    {
        if (_engineService.State != EngineState.Idle)
        {
            return Conflict("Engine is already running.");
        }

        await _engineService.StartWorldGenAsync(request.Seed, request.CellCount);
        return Accepted();
    }
}