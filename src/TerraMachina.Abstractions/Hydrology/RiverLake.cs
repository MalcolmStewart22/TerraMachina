using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Hydrology;

public class RiverLake : IRiverNode
{
    public List<int> OccupoedCellIds { get; init; } = new();
    public List<int> EdgeCellIds { get; init; } = new();
    public int NodeId { get; init; }
    public float FlowRate { get; set; }
    public float InflowRate { get; set; }
    public List<IRiverNode> PreviousNodes { get; set; } = new();
    public IRiverNode? NextNode { get; set; }
    /// <summary> Measured in Meters above SeaLevel</summary>
    public float SurfaceElevation { get; set; }
}