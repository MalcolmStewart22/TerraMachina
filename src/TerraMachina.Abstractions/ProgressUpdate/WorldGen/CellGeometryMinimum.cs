using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TerraMachina.Abstractions.ProgressUpdate.WorldGen;

public  class CellGeometryMinimum
{
    public int ID { get; init; }
    public Vector3[] V { get; init; } = new Vector3[3];

    public CellGeometryMinimum(CellGeometry geo)
    {
        ID = geo.CellId;
        V[0] = geo.Vertices[0];
        V[1] = geo.Vertices[1];
        V[2] = geo.Vertices[2];
    }

}
