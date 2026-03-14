using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TerraMachina.Abstractions.Circulation;

/// <summary>
/// A deep-layer ocean node. Deep nodes carry the slow, cold, density-driven
/// return flow of thermohaline circulation. They receive no wind forcing —
/// flow direction is determined entirely by pressure gradients, which are
/// shaped by bathymetry (cell elevation below sea level) and the density
/// of the water column above.
///
/// Deep nodes flow until they reach an upwelling condition — typically where
/// cold deep water meets a warmer region and rises back to the surface,
/// rejoining surface circulation via UpwellingNeighbors.
/// </summary>
public class DeepOceanNode : IOceanNode
{
    public int NodeId { get; init; }
    public int[] CellIds { get; init; } = [];
    public float Temperature { get; set; }
    public float Salinity { get; set; }
    public float Pressure { get; set; }
    public Vector2 FlowDirection { get; set; }
    public List<IOceanNode> Next { get; set; } = [];
    public List<IOceanNode> Previous { get; set; } = [];
    public List<IOceanNode> LateralNeighbors { get; init;} = [];

    public required SurfaceOceanNode UpwellingNeighbor { get; init;}
    public float AverageDepth { get; set; }
}


