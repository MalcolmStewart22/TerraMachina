using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Enums;
/// <summary>
/// Classifies naturally occurring plant-based resources that can be harvested from a cell.
/// Flora is derived during world generation from a combination of ClimateBiome, SoilType,
/// temperature, and rainfall, and stored as a collection of abundance and quality values
/// on CellResourceData.
///
/// Scope is limited to wild, naturally occurring flora representing the pre-human state
/// of the world.
/// </summary>
public enum FloraResource
{
    Softwood,
    Hardwood,
    MedicinalHerbs,
    Narcotics,
    Bamboo,
    Flax,
    Cotton,
    Hemp,
    Jute,
    EarthTonePlants,
    RedTonePlants,
    YellowTonePlants,
    BlueTonePlants,
    PurpleTonePlants,
    Resin,
    Aromatics,
    CulinaryHerbs,
    Spices,
    WildGrains,
    WildLegumes,
    Nuts,
    RootsAndTubers,
    WildVines, 
    WildOrchardFruits,
    WildCitrus,
    WildTropicalFruits,
    OilPlants,
    AquaticVegetation
}
