using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Circulation;
/// <summary>
/// Contains the complete atmospheric circulation system for the world.
///
/// The graph topology is stored once and shared across all seasonal snapshots.
/// Seasonal variation is expressed through four AtmosphericSnapshots representing
/// the solstices and equinoxes. WorldSim interpolates between these known
/// equilibrium states as the simulation date progresses through the year,
/// updating each snapshot as it passes through that seasonal point so that
/// long term climate shifts — including ice ages — are naturally expressed
/// as drift in the baseline seasonal states rather than scripted events.
/// </summary>
public class AtmosphericCirculation
{
    public List<IAtmosphericNode> Nodes { get; init; } = [];

    public AtmosphericSnapshot WinterSolstice { get; init; } = new();

    public AtmosphericSnapshot SummerSolstice { get; init; } = new();

    public AtmosphericSnapshot VernalEquinox { get; init; } = new();

    public AtmosphericSnapshot AutumnalEquinox { get; init; } = new();
}