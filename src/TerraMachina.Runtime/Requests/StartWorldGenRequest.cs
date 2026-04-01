using TerraMachina.Abstractions.Parameters;

namespace TerraMachina.Runtime.Requests;

public class StartWorldGenRequest
{
    public GeometryParameters Geometry {  get; init; }
}