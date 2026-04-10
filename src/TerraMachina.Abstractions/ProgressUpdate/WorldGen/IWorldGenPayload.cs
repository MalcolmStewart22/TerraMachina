using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using TerraMachina.Abstractions.ProgressUpdate.WorldGen.Geometry;
using TerraMachina.Abstractions.ProgressUpdate.WorldGen.Tectonics;

namespace TerraMachina.Abstractions.ProgressUpdate.WorldGen;

[JsonPolymorphic]
[JsonDerivedType(typeof(GeometryProgressPayload), "geometry")]
[JsonDerivedType(typeof(TectonicsProgressPayload), "tectonics")]
public interface IWorldGenPayload
{
}
