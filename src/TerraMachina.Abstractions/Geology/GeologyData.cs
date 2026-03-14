using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Geology;

public class GeologyData
{
    public required List<TectonicPlate> Plates { get; init; }
    public required List<PlateBoundary> Boundaries { get; init; }
}
