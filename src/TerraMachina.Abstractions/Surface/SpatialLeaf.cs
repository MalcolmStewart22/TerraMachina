using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Surface;

public class SpatialLeaf : SpatialNode
{
    public Cell Cell { get; init; }

    public SpatialLeaf(Cell cell)
    {
        Cell = cell;
    }
}
