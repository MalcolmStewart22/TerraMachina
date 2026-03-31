namespace TerraMachina.Runtime.Services;

public interface IEngineService
{
    EngineState State { get; set; }
    Task StartWorldGenAsync(int seed, int subdivisionLevel);
}