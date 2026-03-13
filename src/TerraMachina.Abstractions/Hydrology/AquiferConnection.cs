using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Hydrology;

public class AquiferConnection : IAquiferNode
{
    public int NodeId { get; init; }
    public List<int> SurfaceCellsById { get; init; } = new();
    public List<IAquiferNode> ConnectedNodes { get; init; } = new();
    public float Pressure { get; set; }
    public int? CurrentReceiverById { get; set; }
}