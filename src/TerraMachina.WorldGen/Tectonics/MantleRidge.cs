using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.WorldGen.Tectonics;

public class MantleRidge
{
    public int RidgeId { get; init; }
    public int PlumeAId { get; init; }
    public int PlumeBId { get; init; }
    public int RidgeWidth { get; init; }
    public List<int> CenterCellIds { get; init; } = new();
    public List<int> ContainedCellIds { get; init; } = new();
}
