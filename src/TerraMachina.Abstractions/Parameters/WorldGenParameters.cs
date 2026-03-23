using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Parameters;

public class WorldGenParameters
{
    public int Seed { get; init; }
    public int SubdivisionLevels { get; init; }

    public WorldGenParameters(int seed, int subdivisionLevels)
    {
        Seed = seed;
        SubdivisionLevels = subdivisionLevels;
    }
}
