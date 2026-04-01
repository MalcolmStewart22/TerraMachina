using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Geology;
/// <summary>
/// Represents a tectonic plate as a named region of cells moving at a scalar velocity.
/// Plate direction is used during world generation to classify boundaries and is not stored.
/// Ongoing simulation uses speed to drive stress accumulation at boundaries.
/// </summary>
public class Plate
{
    public int PlateID { get; init; }
    public List<int> ContainedCellIds { get; init; } = new();
    public float Speed { get; set; }

}
