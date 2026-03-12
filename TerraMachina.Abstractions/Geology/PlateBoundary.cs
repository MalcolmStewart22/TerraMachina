using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Geology;
/// <summary>
/// Represents the boundary between two tectonic plates as a set of cells along their shared edge.
/// Stress accumulates at Convergent and Transform boundaries over simulation time.
/// ConvergenceAngle is only relevant for Convergent boundaries and is null otherwise.
/// </summary>
public class PlateBoundary
{
    public int BoundaryId { get; init; }
    public TectonicPlate? PlateA { get; init; }
    public TectonicPlate? PlateB { get; init; }
    public int[] CellIds { get; init; } = [];
    public BoundaryType BoundaryType { get; init; }
    public ConvergentType? ConvergentAngle { get; init; }
    public float Stress { get; set; }
}
