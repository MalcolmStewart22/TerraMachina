using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using TerraMachina.Abstractions.ProgressUpdate.WorldGen.Geometry;

namespace TerraMachina.Abstractions.ProgressUpdate.WorldGen;

[JsonDerivedType(typeof(GeometryProgressPayload), "geometry")]
public interface IWorldGenPayload
{
}
