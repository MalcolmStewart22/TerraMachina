using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Hydrology;

public class RiverSegment : IRiverNode
{
    public int CellId { get; init; }
    public int NodeId { get; init; }
    public float FlowRate { get; set; }
    public List<IRiverNode> PreviousNodes { get; set; } = new();
    public IRiverNode? NextNode { get; set; }
}