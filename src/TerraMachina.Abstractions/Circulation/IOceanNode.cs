using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TerraMachina.Abstractions.Circulation;
/// <summary>
/// Represents a single node in the global oceanic circulation graph.
///
/// The ocean is modeled as a directed graph of nodes rather than a collection
/// of named currents. Named currents (e.g. "North Atlantic Current") are emergent
/// patterns in this graph, not stored objects. This allows branching, merging, and
/// full connectivity without artificial current boundaries.
///
/// Each node covers a group of ocean cells and maintains cached derived values
/// (Pressure, FlowDirection, Next) that are computed from physical state and
/// invalidated when Temperature or Salinity change.
/// </summary>
public interface IOceanNode
{
    int NodeId { get; init; }
    int[] CellIds { get; }
    float Temperature { get; set; }
    float Salinity { get; set; }
    Vector2 FlowDirection { get; set; }
    List<IOceanNode> Next { get; set; }
    List<IOceanNode> Previous { get; set; }
    List<IOceanNode> LateralNeighbors { get; }
}
