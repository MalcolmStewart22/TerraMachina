using Microsoft.AspNetCore.SignalR;
using TerraMachina.Abstractions.Circulation;
using TerraMachina.Abstractions.Geology;
using TerraMachina.Abstractions.Hydrology;
using TerraMachina.Abstractions.Parameters;
using TerraMachina.Abstractions.ProgressUpdate.WorldGen;
using TerraMachina.Abstractions.ProgressUpdate.WorldGen.Enums;
using TerraMachina.Abstractions.Surface;
using TerraMachina.Abstractions.World;
using TerraMachina.Runtime.Hubs;
using TerraMachina.Runtime.Requests;
using TerraMachina.WorldGen;

namespace TerraMachina.Runtime.Services;

// TODO: add method for cancellation.

public class EngineService : IEngineService
{
    private readonly IHubContext<EngineHub> _hubContext;
    public World _world;
    private WorldGenRunner? _wgRunner;
    private IProgress<WorldGenProgressUpdate>? _progress;
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

    #region IENGINE Fire and Forgets
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
        _ = SetupWorldGenAsync(parameters);

        return Task.CompletedTask;
    }
    public Task StartWorldGenPhaseAsync(int phase)
    {
        if (State != EngineState.Ready)
        {
            throw new InvalidOperationException("Engine is Busy!");
        }
        Console.WriteLine("Test!!" + phase);
        _ = RunWorldGenPhaseAsync(phase);

        return Task.CompletedTask;
    }

    public Task ResetEngineAsync()
    {
        State = EngineState.Idle;
        
        if(_progress != null)
        {
            _progress.Report(new WorldGenProgressUpdate
            {
                CurrentStage = WorldGenStageTypes.Reset,
            });
        }

        return Task.CompletedTask;
    }
    #endregion



    #region WorldGen Calls
    private async Task SetupWorldGenAsync(WorldGenParameters parameters)
    {
        _progress = new SynchronousProgress<WorldGenProgressUpdate>(async update =>
        {
            await _hubContext.Clients.All.SendAsync("WorldGenProgress", update);
        });

        _wgRunner = new WorldGenRunner(parameters, _progress, _world);

        await RunWorldGenPhaseAsync(1);

        await _hubContext.Clients.All.SendAsync("RUNTIME: WorldGen Ready!");
    }


    private async Task RunWorldGenPhaseAsync(int phase)
    {
        State = EngineState.Generating;

        try
        {
            switch(phase)
            {
                case 1: //Geometry
                    if(_wgRunner != null)
                    {
                        Console.WriteLine("RUNTIME: WorldGen Geometry Starting!");
                        await _wgRunner.RunGeometryAsync();
                        Console.WriteLine("RUNTIME: WorldGen Geometry Completed!");
                        Console.WriteLine($"RUNTIME: Cells generated: {_world.Surface.Cells.Count()}");
                    }
                    else
                    {
                        throw new ArgumentException("RUNTIME: WORLDGEN HAS NOT BEEN STARTED!");
                    }
                    break;
                case 2: //Tectonics
                    if (_wgRunner != null && _world.Surface.Cells.Count > 0)
                    {
                        Console.WriteLine("RUNTIME: WorldGen Tectonics Starting!");
                        await _wgRunner.RunTectonicsAsync();
                        Console.WriteLine("RUNTIME: WorldGen Tectonics Completed!");
                    }
                    else
                    {
                        throw new ArgumentException("RUNTIME: WORLDGEN NOT READY FOR TECTONICS!");
                    }
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");

            State = EngineState.Idle;
            await _hubContext.Clients.All.SendAsync("WorldGenFailed", ex.Message);
        }

        State = EngineState.Ready;
    }
    #endregion

}