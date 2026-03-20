using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.ProgressUpdate.WorldGen;

public class GeometryProgressPayload : IWorldGenPayload
{
    public List<CellGeometry> Sphere { get; init; } = new();

    public GeometryProgressPayload(CellMap cellMap)
    {
        foreach (Cell c in cellMap.Cells)
        {
            Sphere.Add(c.Geometry);
        }
    }
}
