using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.WorldGen.Geometry;

public class Triangle
{
    public int a;
    public int b;
    public int c;
    public int Level;
    public List<Triangle> Children { get; set; } = new();

    public Triangle(int x, int y, int z, int level)
    {
        a = x;
        b = y;
        c = z;
        Level = level;
    }
}
