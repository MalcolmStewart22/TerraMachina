using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Circulation;
/// <summary>
/// Represents a single node in the atmospheric circulation graph.
///
/// Atmospheric nodes group multiple triangular cells into a unit large enough
/// for circulation physics to be meaningful. The graph topology — cell membership
/// and neighbor relationships — is fixed after WorldGen and shared across all
/// seasonal snapshots. Only the circulation state (temperature, humidity, wind
/// force, flow path) varies between snapshots.
///
/// Nodes are partitioned to ensure a minimum of 6 lateral neighbors, providing
/// enough directional resolution for realistic wind patterns. This requires
/// grouping enough triangular cells that the node's perimeter touches at least
/// 6 other nodes.
/// </summary>
public interface IAtmosphericNode
{
    int NodeId { get; init; }
    int[] CellIds { get; }
    List<IAtmosphericNode> LateralNeighbors { get; init; }
}
