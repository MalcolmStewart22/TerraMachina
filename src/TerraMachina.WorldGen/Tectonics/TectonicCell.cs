using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using TerraMachina.Abstractions.Surface;

namespace TerraMachina.WorldGen.Tectonics;

public class TectonicCell
{
    public CellGeometry Geometry { get; init; }
    public float Elevation { get; set; }
    public Vector3 EulerPoleDerivedDirection { get; set; }
    public Vector3 CurrentDirection { get; set; }
    public float Speed { get; set; }
    public bool ActiveVolcano { get; set; }

    public TectonicCell(CellGeometry geometry)
    {
        Geometry = geometry;
    }
}
