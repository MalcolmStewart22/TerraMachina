using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Geology;

public class GeologyData
{
    public List<TectonicPlate> Plates { get; init; }
    public List<PlateBoundary> Boundaries { get; init; }
}
