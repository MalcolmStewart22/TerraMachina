using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using TerraMachina.Abstractions.Geology;

namespace TerraMachina.WorldGen.Tectonics;

public class TectonicPlate
{
    public static int NextId = 1;
    public int PlateID { get; init; }
    public string Name { get; init; }
    public List<int> ContainedCellIds { get; init; } = new();
    public Vector3 EulerPolePosition { get; set; }
    public List<int> BoundaryCells { get; init; } = new();
    public List<TectonicBoundary> Boundaries { get; init; } = new();
    public float Density { get; set; }

    public TectonicPlate(string name, List<TectonicCell> cells, Vector3 pole)
    {
        PlateID = NextId;
        NextId++;
        Name = name;

        foreach(var cell in cells)
        {
            ContainedCellIds.Add(cell.Geometry.CellId);
            cell.PlateId = PlateID;
        }
    }
}
