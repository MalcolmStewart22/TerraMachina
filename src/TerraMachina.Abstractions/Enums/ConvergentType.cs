using System;
using System.Collections.Generic;
using System.Text;



namespace TerraMachina.Abstractions.Enums;

/// <summary>
/// Describes the angle of plate convergence at a convergent boundary.
/// Only applicable where BoundaryType is Convergent.
/// Angle classification based on standard geological literature.
/// Oblique convergence produces hybrid uplift and lateral displacement,
/// while head-on convergence maximises orogenic activity.
/// </summary>

public enum ConvergentType
{
    HeadOn,
    Oblique,
    Glancing
     
}
