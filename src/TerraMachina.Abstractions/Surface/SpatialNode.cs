using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TerraMachina.Abstractions.Surface;

public class SpatialNode
{
    public int NodeID { get; set; }
    public Vector3[] Vertices { get; init; } = new Vector3[3];
    public List<SpatialNode> Children { get; init; } = new();
}
