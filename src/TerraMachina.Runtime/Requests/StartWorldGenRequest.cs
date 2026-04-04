using TerraMachina.Abstractions.Parameters;

namespace TerraMachina.Runtime.Requests;

public class StartWorldGenRequest
{
    public required GeometryParameters Geometry {  get; init; }
    public required TectonicParameters Tectonic {  get; init; }
}