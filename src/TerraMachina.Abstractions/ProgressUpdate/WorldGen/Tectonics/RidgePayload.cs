using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.ProgressUpdate.WorldGen.Tectonics;

public class RidgePayload
{
    public int RidgeId { get; set; }
    public List<TectonicCellMinimum> ContainedCells { get; set; } = new();

    public RidgePayload(int id, List<TectonicCellMinimum> cells)
    {
        RidgeId = id;
        ContainedCells = [.. cells];
    }
}
