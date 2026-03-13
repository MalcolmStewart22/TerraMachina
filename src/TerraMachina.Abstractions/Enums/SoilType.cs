namespace TerraMachina.Abstractions.Enums;

/// <summary>
/// Classifies the primary soil composition of a cell as determined during world generation.
/// Soil type is derived from BedrockType in combination with climate and vegetation data,
/// and is stored as an initial equilibrium state from which the simulation may deviate over time.
/// 
/// Soil type seeds several simulation systems including fertility, hydrology and drainage,
/// buildability, vegetation type, agriculture type, and erosion susceptibility.
/// </summary>

public enum SoilType
{
    Clay,
    Sandy,
    Loam,
    Peat,
    Silt,
    Rocky
}
