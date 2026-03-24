using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TerraMachina.Abstractions.ProgressUpdate.WorldGen;

[JsonDerivedType(typeof(GeometryProgressPayload), "geometry")]
public interface IWorldGenPayload
{
}
