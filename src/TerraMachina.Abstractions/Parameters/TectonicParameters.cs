using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Parameters;
/// <summary>
/// Configuration parameters for the tectonic simulation phase of world generation.
///
/// Design Choices:
/// - Seed plumes are intentionally smaller than geologically accurate (user defined via
///   MinPlumeSize/MaxPlumeSize) to produce more varied ridge geometry and plate diversity.
/// - Later plumes target the geologically accurate ~2500km diameter surface expression.
/// - Ridge width is uniform across the entire ridge system. In reality ridge width varies
///   with spreading rate, but uniform width is sufficient for simulation purposes.
/// - Plates are segmented from ridge geometry using Euler pole convergence rather than
///   being explicitly defined, allowing plate count and size to emerge naturally.
/// - Volcanoes are treated as surface pressure points within a plume rather than
///   independent geological features.
/// - Elevation is not simulated during the plate fill phase. Crust is treated as
///   unformed and flat until fill completes, at which point ridge and plume elevations
///   are applied before tectonic simulation begins.
/// - Ocean designation is deferred entirely until after tectonics completes. The tectonic
///   phase operates purely in terms of elevation relative to zero with no sea level.
/// - The ridge system plume cap reuses TotalSeedPlumes as its maximum, keeping the
///   system complexity bounded without introducing a separate parameter.
/// - Tectonic duration is expressed in ticks where each tick represents approximately
///   2 million years, derived from the time required for a plate to advance one cell
///   width at an average spreading rate of 3cm per year.
/// - All measurements are in Kilometers
/// </summary>
public class TectonicParameters
{
    //Base
    public int PlanetRadius { get; set; } 
    public int TectonicSimulationLength { get; set; }
    //Mantle Activity
    public int MaxPlumeSize { get; set; }
    public int MinPlumeSize { get; set; }
    public int TotalSeedPlumes { get; set; }
    public float PlumeCenterSize { get; set; }
    public float PlumeFrequency { get; set; }
    public float PlumeBaseStrength { get; set; } //normalized 0-1
    public float PlumeConnectivityDistance { get; set; } //as a percentage of plume radius
    public int PlumeMaxAge { get; set; }
    public int RidgeWidth { get; set; }
    public float VolcanoFrequency { get; set; }
    //Plates
    public float EulerPoleTolerance { get; set; }
    public float BasePlateSpeed { get; set; }
}
