using TerraMachina.Runtime.Requests;

namespace TerraMachina.Runtime.Services;

public interface IEngineService
{
    EngineState State { get; set; }
    Task StartWorldGenAsync(StartWorldGenRequest request);
}