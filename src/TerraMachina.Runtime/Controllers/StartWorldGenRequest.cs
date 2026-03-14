namespace TerraMachina.Runtime.Controllers;

public class StartWorldGenRequest
{
    public int Seed { get; init; }
    public int CellCount { get; init; }
}