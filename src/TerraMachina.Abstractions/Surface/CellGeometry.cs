using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TerraMachina.Abstractions.Surface;

public class CellGeometry
{
    public int CellId { get; init; }
    public Vector3 Position { get; init; }
    public int[] NeighborsById { get; init; } = new int[3];
    public Vector3[] Vertices { get; init; } = new Vector3[3];


    public CellGeometry(int id, Vector3 pos, Vector3[] vertices)
    {
        CellId = id;
        Position = pos;
        Vertices = vertices.ToArray();
    }
}
