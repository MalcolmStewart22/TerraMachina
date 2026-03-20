using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.ProgressUpdate.WorldGen;

public class GeometryProgressPayload : IWorldGenPayload
{
    public required List<CellGeometry> Sphere { get; init; }
}
