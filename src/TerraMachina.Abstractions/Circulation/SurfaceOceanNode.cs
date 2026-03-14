using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TerraMachina.Abstractions.Circulation;
/// <summary>
/// A surface-layer ocean node. Surface nodes are the wind-driven layer of
/// oceanic circulation — they receive forcing from atmospheric circulation
/// and are the origin point of evaporation-driven salinity increases.
///
/// When a surface node becomes sufficiently cold and saline, its water density
/// exceeds that of the water below it and it transitions to deep circulation
/// via its DownwellingNeighbors. This sinking is the engine of thermohaline
/// circulation.
/// </summary>
public class SurfaceOceanNode : IOceanNode
{
    public int NodeId { get; init; }
    public int[] CellIds { get; init; } = [];
    public float Temperature { get; set; }
    public float Salinity { get; set; }
    public float FlowForce { get; set; }
    public Vector2 FlowDirection { get; set; }
    public List<IOceanNode> Next { get; set; } = [];
    public List<IOceanNode> Previous { get; set; } = [];
    public List<IOceanNode> LateralNeighbors { get; init;} = [];
    public required DeepOceanNode DownwellingNeighbor { get; init;}
}
