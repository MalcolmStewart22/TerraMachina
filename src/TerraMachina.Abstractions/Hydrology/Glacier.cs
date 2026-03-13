using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Hydrology;

public class Glacier
{
    public int GlacierId { get; init; }
    public List<int> OccupiedCellIds { get; init; } = new();
    public List<int> AblationZone { get; init;  } = new();
    public List<int> AccumulationZone { get; init; } = new();
    /// <summary> Measured in Meters above sea level</summary>
    public int CurrentHeight { get; set; }
}