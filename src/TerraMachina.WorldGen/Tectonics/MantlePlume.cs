using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TerraMachina.WorldGen.Tectonics;

public class MantlePlume
{
    public int PlumeID {  get; init; }
    public Vector3 Center { get; init; }
    public List<int> CellIds { get; init; } = new();
    public List<int> CenterCellIds { get; init; } = new();
    public int Age {  get; set; }
    public int RadiusKm { get; init; }

    //seed plumes
    public MantlePlume(int id, Vector3 center, int size)
    {
        PlumeID = id;
        Center = center;
        Age = 0;
        RadiusKm = size;
    }
}
