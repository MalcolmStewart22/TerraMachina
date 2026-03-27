using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Surface;

public class CellMap
{
    public List<Cell> Cells { get; init; } = new();
    public Dictionary<int, Cell> CellById { get; init; } = new();

    public List<SpatialNode> WorldTree {  get; init; } = new(); //used for spatial lookups and viewport streaming
}