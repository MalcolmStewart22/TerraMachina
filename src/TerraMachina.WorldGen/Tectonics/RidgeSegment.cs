using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TerraMachina.WorldGen.Tectonics;

public class RidgeSegment
{
    static int NextID = 1;
    public int SegmentId { get; init; }
    public Vector3 StartPoint { get; set; }
    public Vector3 EndPoint { get; set; }
    public Vector3 Normal { get; set; }

    public RidgeSegment(Vector3 startPoint, Vector3 endPoint, Vector3 normal)
    {
        StartPoint = startPoint;
        EndPoint = endPoint;
        Normal = normal;
        SegmentId = NextID;
        NextID++;
    }
}
