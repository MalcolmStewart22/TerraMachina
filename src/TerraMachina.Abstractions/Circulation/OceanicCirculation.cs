using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Circulation;
/// <summary>
/// Contains the complete oceanic circulation graph for the world.
///
/// The ocean is represented as a single connected directed graph of IOceanNodes
/// rather than a collection of named currents. Named patterns such as the
/// North Atlantic Current or the Antarctic Circumpolar Current are emergent
/// properties of this graph — identifiable by traversal when needed, but not
/// stored as discrete objects. This avoids artificial boundaries at locations
/// where real currents branch, merge, or share water.
///
/// The full node list is the authoritative source for all oceanic state.
/// WorldSim operates directly on nodes, recalculating Pressure and FlowDirection
/// only on nodes whose Temperature or Salinity have changed that tick.
/// </summary>
public class OceanicCirculation
{
    public List<IOceanNode> Nodes { get; init; } = [];
}
