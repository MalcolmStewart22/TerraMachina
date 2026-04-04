using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.ProgressUpdate.WorldGen.Tectonics;

public class TectonicsProgressPayload : IWorldGenPayload
{
    public List<PlumePayload>? Plumes { get; set; }
    public List<RidgePayload>? Ridges { get; set; }
    public Dictionary<int, string>? Plates { get; set; } = new();
    public List<TectonicCellMinimum>? TectonicCells { get; set; }
}
