using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.ProgressUpdate.WorldGen.Tectonics;

public class PlumePayload
{
    public int PlumeId { get; set; }
    public List<TectonicCellMinimum> ContainedCells { get; set; } = new();
    public List<TectonicCellMinimum> CenterCells { get; set; } = new();

    public PlumePayload(int id, List<TectonicCellMinimum> cells, List<TectonicCellMinimum> centerCells)
    {
        PlumeId = id;
        ContainedCells = [..cells];
        CenterCells = [.. centerCells];
    }
}
