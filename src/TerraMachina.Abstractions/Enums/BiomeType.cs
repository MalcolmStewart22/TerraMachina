using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Enums;

/// <summary>
/// Biome classification used throughout TerraMachina's world generation and simulation systems.
/// 
/// Based on Whittaker's biome classification system (Whittaker, R.H., 1975. Communities and Ecosystems.
/// MacMillan, New York), which defines biomes primarily along axes of mean annual temperature
/// and mean annual precipitation.
/// 
/// The following modifications were made to suit the needs of this particular simulation and it's audience:
/// - Tropical Seasonal Forest and Savanna are separated, distinguished by rainfall seasonality
/// - Temperate Grassland and Cold Desert are separated by annual rainfall threshold
/// - Hot Desert replaces Whittaker's Subtropical Desert for clarity and intuitive naming
/// - Woodland/Shrubland is narrowed to Shrubland, representing the transitional zone
///   between forest and grassland where rainfall supports woody plants but not closed canopy
/// - Wetlands and Ice Cap are added as hydrologically and climatically distinct zones
///   not adequately captured by Whittaker's temperature/precipitation axes alone
/// - Wetlands are split into three temperature grades (Tropical, Temperate, Boreal) rather
///   than treated as a single biome, consistent with the temperature-graded treatment of
///   forests. Boreal Wetlands are treated as distinct from Temperate due to permafrost,
///   which fundamentally alters soil composition, water retention, and vegetation.
/// </summary>
public enum BiomeType
{
    TropicalRainforest,
    TropicalSeasonalRainforest,
    Savanna,
    TemperateRainforest,
    TemperateSeasonalRainforest,
    BorealForest,
    Shrubland,
    TropicalWetland,
    TemperateWetland,
    BorealWetland,
    TemperateGrassland,
    HotDesert,
    ColdDesert,
    Tundra,
    PolarDesert
}
