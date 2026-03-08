using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Circulation;
/// <summary>
/// The seasonal state of a single atmospheric node. Separated from the node's
/// structural data so that four seasonal snapshots can share one graph topology
/// without duplicating cell memberships and neighbor relationships.
///
/// Next and Previous are part of state rather than structure because circulation
/// paths shift between seasons — jet streams move to different latitudes, trade
/// wind boundaries shift with the ITCZ, and monsoon systems reverse entirely.
/// The linked list of flow paths is a seasonal equilibrium, not a fixed property
/// of the node's position.
/// </summary>
public class AtmosphericNodeState
{
    public float Temperature { get; set; }
    public float Humidity { get; set; }
    public float WindForce { get; set; }
    public List<IAtmosphericNode> Next { get; set; } = [];
    public List<IAtmosphericNode> Previous { get; set; } = [];
}
