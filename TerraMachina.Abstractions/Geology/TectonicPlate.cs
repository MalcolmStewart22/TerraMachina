using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Geology;
/// <summary>
/// Represents a tectonic plate as a named region of cells moving at a scalar velocity.
/// Plate direction is used during world generation to classify boundaries and is not stored.
/// Ongoing simulation uses velocity to drive stress accumulation at boundaries.
/// </summary>
public class TectonicPlate
{
    public int PlateID { get; init; }
    public int[] CellIds { get; init; } = [];
    public float Velocity { get; init; }

}
