using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Circulation;
/// <summary>
/// A surface-layer atmospheric node. Surface nodes represent the lowest layer
/// of the troposphere where weather systems develop and where circulation
/// directly influences cell climate.
///
/// Surface flow is dampened by friction with the underlying terrain — land
/// produces significantly more friction than ocean, and topography amplifies
/// this further. Friction coefficients are stored on WorldConstants rather
/// than per node as they are physical properties of the world, not of
/// individual nodes.
///
/// Wind direction and force at this layer are what drive surface ocean nodes
/// and produce the PrevailingWind derivative on CellClimateData.
/// </summary>
public class SurfaceAtmosphericNode : IAtmosphericNode
{
    public int[] CellIds { get; init; } = [];
    public List<IAtmosphericNode> LateralNeighbors { get; init; } = [];
    public UpperAtmosphericNode UpperNeighbor { get; init; } = null!;
}
