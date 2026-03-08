using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Circulation;
/// <summary>
/// The atmospheric circulation state at a specific seasonal equilibrium point.
/// Four snapshots are maintained — winter solstice, summer solstice, vernal
/// equinox, and autumnal equinox — providing enough sample points for WorldSim
/// to interpolate realistic seasonal transitions including monsoon onset,
/// hurricane season windows, and flood timing cycles.
///
/// Snapshots store only state values keyed to node identity. The graph topology
/// — cell memberships and neighbor relationships — lives once on
/// AtmosphericCirculation and is shared across all snapshots.
/// </summary>
public class AtmosphericSnapshot
{
    public Dictionary<IAtmosphericNode, AtmosphericNodeState> NodeStates { get; init; } = [];
}
