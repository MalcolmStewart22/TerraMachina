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
using TerraMachina.Runtime.Requests;
using TerraMachina.WorldGen;

namespace TerraMachina.Runtime.Services;

public class EngineService : IEngineService
{
    private readonly IHubContext<EngineHub> _hubContext;
    public World _world;

    public EngineState State { get; set; } = EngineState.Idle;

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

    public Task StartWorldGenAsync(StartWorldGenRequest request)
    {
        if (State != EngineState.Idle)
        {
            throw new InvalidOperationException("Engine is not idle.");
        }

        State = EngineState.Generating;
       
        _world = new World
            (
                new CellMap(),
                new GeologyData(),
                new HydrologyData(),
                new CirculationSystems()
            );

        WorldGenParameters parameters = new WorldGenParameters(request.Geometry, request.Tectonic);
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

            WorldGenRunner wgRunner = new WorldGenRunner(parameters, progress, _world);

            await wgRunner.RunGeometryAsync();

            State = EngineState.Waiting;
            Console.WriteLine("WorldGen Geometry Completed!");
            Console.WriteLine($"Cells generated: {_world.Surface.Cells.Count}");
            while(State == EngineState.Waiting)
            {
                await Task.Delay(50);
            }
            await wgRunner.RunTectonicsAsync();

            await _hubContext.Clients.All.SendAsync("WorldGenComplete");
        }
        catch (Exception ex)
        {
            State = EngineState.Idle;
            await _hubContext.Clients.All.SendAsync("WorldGenFailed", ex.Message);
        }
    }
}