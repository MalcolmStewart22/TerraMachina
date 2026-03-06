using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Enums;

/// <summary>
/// Classifies naturally occurring geological resources that can be harvested from a cell.
/// Resources are derived during world generation from a combination of BedrockType,
/// SoilType, elevation, and geological conditions, and stored as a collection of abundance and quality values
/// on CellResourceData.
/// 
/// Scope is limited to resources with meaningful pre-gunpowder, medieval-era significance.
/// Gunpowder precursors such as saltpeter and sulfur are intentionally excluded until
/// the simulation is extended to a later technological era.
/// </summary>
public enum GeologicalResource
{
    PreciousMetals,
    Copper,
    Tin,
    Iron,
    Lead,
    Gems,
    Obsidian,
    Granite,
    Slate,
    Limestone,
    Sandstone,
    Shale,
    Marble,
    Clay,
    Salt,
    BlackOil,
    Pigments,
    Coal,
    Peat
}
