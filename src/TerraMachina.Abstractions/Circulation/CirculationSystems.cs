namespace TerraMachina.Abstractions.Circulation;

/// <summary>
/// Circulation is the primary driver of climate — oceanic circulation
/// determines heat distribution across the world's oceans, while atmospheric
/// circulation determines wind patterns and rainfall. Together they produce
/// the baseline climate state that WorldGen outputs and that WorldSim
/// simulates variance on top of.
///
/// Split into oceanic and atmospheric subsystems as these operate through
/// fundamentally different physical mechanisms, at different speeds, and
/// are perturbed by different simulation events.
/// </summary>
public class CirculationSystems
{
    public OceanicCirculation Ocean { get; init; } = new();
    public AtmosphericCirculation Atmosphere { get; init; } = new();
}