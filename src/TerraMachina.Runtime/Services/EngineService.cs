using Microsoft.AspNetCore.SignalR;
using TerraMachina.Runtime.Hubs;

namespace TerraMachina.Runtime.Services;

public class EngineService : IEngineService
{
    private readonly IHubContext<EngineHub> _hubContext;

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

        _ = RunWorldGenAsync(seed, subdivisionLevel);

        return Task.CompletedTask;
    }

    private async Task RunWorldGenAsync(int seed, int subdivisionLevel)
    {
        try
        {
            var progress = new Progress<string>(update =>
            {
                _ = _hubContext.Clients.All.SendAsync("WorldGenProgress", update);
            });

            await Task.Delay(5000); // placeholder simulating work

            State = EngineState.Ready;
            await _hubContext.Clients.All.SendAsync("WorldGenComplete");
        }
        catch (Exception ex)
        {
            State = EngineState.Idle;
            await _hubContext.Clients.All.SendAsync("WorldGenFailed", ex.Message);
        }
    }
}