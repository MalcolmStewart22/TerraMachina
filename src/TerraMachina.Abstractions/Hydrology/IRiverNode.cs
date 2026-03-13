using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Hydrology;

public interface IRiverNode
{
    public int NodeId { get; init; }
    public float FlowRate { get; set; }
    public List<IRiverNode> PreviousNodes { get; set; }
    public IRiverNode? NextNode {  get; set; }
}