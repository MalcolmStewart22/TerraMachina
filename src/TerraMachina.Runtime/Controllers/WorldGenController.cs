using Microsoft.AspNetCore.Mvc;
using TerraMachina.Runtime.Requests;
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

        await _engineService.StartWorldGenAsync(request);
        return Accepted();
    }

    [HttpPost("ready")]
    public async Task<IActionResult> StartNextEnginePhaseAsync(StartNextWorldGenPhaseRequest request)
    {
        if (_engineService.State != EngineState.Ready)
        {
            return Conflict("Engine is busy.");
        }

        Console.WriteLine("RUNTIME: Continue Order Recieved!" + request);
        await _engineService.StartWorldGenPhaseAsync(request.Phase);
        return Accepted();
    }

    [HttpPost("reset")]
    public async Task<IActionResult> ResetEngine()
    {
        if (_engineService.State != EngineState.Ready)
        {
            return Conflict("Engine cannot currently be reset.");
        }
        Console.WriteLine("RUNTIME: RESETING ENGINE!");
        await _engineService.ResetEngineAsync();
        return Accepted();
    }
}