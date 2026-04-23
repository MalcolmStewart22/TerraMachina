using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TerraMachina.WorldGen.Tectonics;

public class MantlePlume
{
    static int NextID = 1;
    public int PlumeID {  get; init; }
    public Vector3 Center { get; init; }
    public List<int> CellIds { get; init; } = new();
    public List<int> CenterCellIds { get; init; } = new();
    public int Age {  get; set; }
    public int RadiusKm { get; init; }

    //seed plumes
    public MantlePlume(Vector3 center, int size)
    {
        PlumeID = NextID;
        NextID++;
        Center = center;
        Age = 0;
        RadiusKm = size;
    }
}
