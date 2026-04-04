using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TerraMachina.WorldGen.Tectonics;

public class RidgeSegment
{
    public Vector3 StartPoint { get; init; }
    public Vector3 EndPoint { get; init; }
    public Vector3 Normal { get; init; }

    public RidgeSegment(Vector3 startPoint, Vector3 endPoint, Vector3 normal)
    {
        StartPoint = startPoint;
        EndPoint = endPoint;
        Normal = normal;
    }
}
