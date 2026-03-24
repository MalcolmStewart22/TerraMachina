using Microsoft.AspNetCore.SignalR;
using TerraMachina.Runtime.Hubs;
using TerraMachina.WorldGen;
using TerraMachina.Abstractions.Parameters;
using TerraMachina.Abstractions.ProgressUpdate;
using TerraMachina.Abstractions.ProgressUpdate.WorldGen;
using TerraMachina.Abstractions.World;

namespace TerraMachina.Runtime.Services;

public class EngineService : IEngineService
{
    private readonly IHubContext<EngineHub> _hubContext;
    public World _world;

    public EngineState State { get; private set; } = EngineState.Idle;

    public EngineService(IHubContext<EngineHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task StartWorldGenAsync(int seed, int subdivisionLevel)
    {
        if (State != EngineState.Idle)
        {
            throw new InvalidOperationException("Engine is not idle.");
        }

        State = EngineState.Generating;

        
        WorldGenParameters parameters = new WorldGenParameters(seed, subdivisionLevel);
        _ = RunWorldGenAsync(parameters);

        return Task.CompletedTask;
    }

    private async Task RunWorldGenAsync(WorldGenParameters parameters)
    {
        try
        {
            var progress = new Progress<WorldGenProgressUpdate>(update =>
            {
                _ = _hubContext.Clients.All.SendAsync("WorldGenProgress", update);
            });

            WorldGenRunner wgRunner = new WorldGenRunner();

            _world = await wgRunner.RunAsync(parameters,progress);

            State = EngineState.Idle;
            Console.WriteLine("World Gen Completed!");
            Console.WriteLine($"Cells generated: {_world.Surface.Cells.Count}");
            await _hubContext.Clients.All.SendAsync("WorldGenComplete");
        }
        catch (Exception ex)
        {
            State = EngineState.Idle;
            await _hubContext.Clients.All.SendAsync("WorldGenFailed", ex.Message);
        }
    }
}