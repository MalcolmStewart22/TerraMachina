using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Parameters;

public class WorldGenParameters
{
    public GeometryParameters GeometryParameters { get; set; }
    public TectonicParameters TectonicParameters { get; set; }
    public WorldGenParameters(GeometryParameters geometryParameters, TectonicParameters tectonicParameters)
    {
        GeometryParameters = geometryParameters;
        TectonicParameters = tectonicParameters;
    }
}
