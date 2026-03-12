using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TerraMachina.Abstractions.Surface;

public class Cell
{
    public int CellId { get; init; }
    public Vector3 Position { get; init; }
    public int[] NeighborsById { get; init; } = new int[3];
    public BiomeType Biome { get; set; }
    //Resources
    public Dictionary<GeologicalResource, ResourceDeposit>? GeologicalResources { get; set; } //Resource -> Quantity, Quality
    public Dictionary<FaunaResource, ResourceDeposit>? FaunaResources { get; set; } //Resource -> Quantity, Quality
    public Dictionary<FloraResource, ResourceDeposit>? FloraResources { get; set; } //Resource -> Quantity, Quality
    //Geology Data
    public int Elevation { get; set; }
    public SoilType Soil { get; set; }
    public BedrockType Bedrock { get; set; }
    //Hydrology Data
    public bool HasRiver { get; set; }
    public int? RiverId { get; set; }
    public bool IsLake { get; set; }
    public int? LakeId { get; set; }
    public float SoilWaterContent { get; set; }
    public int StandingWater { get; set; }
    public int SnowPack { get; set; }
    public bool IsWetland { get; set; }
    public bool IsCoastland { get; set; }
    //Atmosphere Data
    public float Temperature { get; set; }
    public float Humidty { get; set; }
    public Vector2 WindDirection { get; set; }
    public float WindSpeed { get; set; }
}
