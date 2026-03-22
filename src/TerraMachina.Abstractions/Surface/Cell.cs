using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TerraMachina.Abstractions.Surface;

public class Cell
{
    public CellGeometry Geometry { get; init; }
    public bool IsOcean { get; set; }
    public BiomeType Biome { get; set; }
    //Resources
    public Dictionary<GeologicalResource, ResourceDeposit>? GeologicalResources { get; set; } //Resource -> Quantity, Quality
    public Dictionary<FaunaResource, ResourceDeposit>? FaunaResources { get; set; } //Resource -> Quantity, Quality
    public Dictionary<FloraResource, ResourceDeposit>? FloraResources { get; set; } //Resource -> Quantity, Quality
    //Geology Data
    public int Elevation { get; set; }
    public SoilType Soil { get; set; }
    public float CurrentFertility { get; set; }
    public BedrockType Bedrock { get; set; }
    //Hydrology Data
    public int? RiverId { get; set; }
    public int? LakeId { get; set; }
    public float SoilWaterContent { get; set; }
    ///<summary> Measured in Meters</summary>
    public int StandingWater { get; set; }
    ///<summary> Measured in Meters</summary>
    public int SnowPack { get; set; }
    public bool IsWetland { get; set; }
    public bool IsCoastland { get; set; }
    //Atmosphere Data
    public float Temperature { get; set; }
    public float Humidty { get; set; }


    public Cell(CellGeometry geo)
    {
        Geometry = geo;
    }
}
