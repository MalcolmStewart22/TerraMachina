namespace TerraMachina.Runtime.Services;

public interface IEngineService
{
    EngineState State { get; }
    Task StartWorldGenAsync(int seed, int subdivisionLevel);
}