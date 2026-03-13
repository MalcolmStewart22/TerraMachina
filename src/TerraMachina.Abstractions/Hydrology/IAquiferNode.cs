using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Hydrology;

public interface IAquiferNode
{
    public int NodeId { get; init; }
    public List<int> SurfaceCellsById { get; init; }
    public List<IAquiferNode> ConnectedNodes { get; init; }
    public float Pressure { get; set; }
}