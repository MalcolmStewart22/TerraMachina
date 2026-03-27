using Microsoft.AspNetCore.SignalR;
using TerraMachina.Abstractions.Circulation;
using TerraMachina.Abstractions.Geology;
using TerraMachina.Abstractions.Hydrology;
using TerraMachina.Abstractions.Parameters;
using TerraMachina.Abstractions.ProgressUpdate;
using TerraMachina.Abstractions.ProgressUpdate.WorldGen;
using TerraMachina.Abstractions.Surface;
using TerraMachina.Abstractions.World;
using TerraMachina.Runtime.Hubs;
using TerraMachina.WorldGen;

namespace TerraMachina.Runtime.Services;

public class EngineService : IEngineService
{
    private readonly IHubContext<EngineHub> _hubContext;
    public World _world;

    public EngineState State { get; private set; } = EngineState.Idle;

    public EngineService(IHubContext<EngineHub> hubContext)
    {
        _hubContext = hubContext;
        _world = new World
            (
                new CellMap(),
                new GeologyData(),
                new HydrologyData(),
                new CirculationSystems()
            );
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
            var progress = new SynchronousProgress<WorldGenProgressUpdate>(async update =>
            {
                await _hubContext.Clients.All.SendAsync("WorldGenProgress", update);
            });

            WorldGenRunner wgRunner = new WorldGenRunner();

            await wgRunner.RunAsync(parameters,progress, _world);

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