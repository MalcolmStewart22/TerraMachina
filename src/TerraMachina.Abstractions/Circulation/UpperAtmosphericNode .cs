using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Circulation;
/// <summary>
/// An upper-layer atmospheric node representing the top of the troposphere,
/// approximately at jet stream altitude (250-300 hPa).
///
/// Upper nodes are free from surface friction and express the full momentum
/// differential of air that has traveled poleward from the equator. This
/// produces the fast eastward jet streams at cell boundaries and the slower
/// return flows within cells. The upper layer is significantly more stable
/// than the surface layer — its baseline state is largely determined by
/// latitude and planetary rotation, with continent placement and ocean
/// temperatures producing smaller perturbations than at the surface.
///
/// The velocity difference between this node and its surface counterpart
/// is wind shear — a critical factor in storm development and dissipation.
/// </summary>
public class UpperAtmosphericNode : IAtmosphericNode
{
    public int NodeId { get; init; }
    public int[] CellIds { get; init; } = [];
    public List<IAtmosphericNode> LateralNeighbors { get; init; } = [];
    public SurfaceAtmosphericNode LowerNeighbor { get; init; } = null!;
}
