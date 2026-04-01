using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Geology;

public class GeologyData
{
    public List<Plate> Plates { get; init; } = new();
    public List<PlateBoundary> Boundaries { get; init; } = new();
}
