using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.ProgressUpdate.WorldGen.Tectonics;

public class RidgePayload
{
    public int RidgeId { get; set; }
    public List<TectonicCellMinimum> ContainedCells { get; set; } = new();
    public List<TectonicCellMinimum> CenterCells { get; set; } = new();

    public RidgePayload(int id, List<TectonicCellMinimum> cells, List<TectonicCellMinimum> centerCells)
    {
        RidgeId = id;
        ContainedCells = [.. cells];
        CenterCells = [.. centerCells];
    }
}
