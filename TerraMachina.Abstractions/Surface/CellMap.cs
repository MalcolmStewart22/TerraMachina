using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Surface;

public class CellMap
{
    public List<Cell> Cells { get; init; } = new();
    public Dictionary<int, Cell>? CellById { get; init; }
}