using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Parameters;

public class WorldGenParameters
{
    public GeometryParameters GeometryParameters { get; set; }

    public WorldGenParameters(GeometryParameters geometryParameters)
    {
        GeometryParameters = geometryParameters;
    }
}
