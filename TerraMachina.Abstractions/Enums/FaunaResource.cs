using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Enums;
/// <summary>
/// Classifies naturally occurring animal-based resources that can be harvested from a cell.
/// Fauna is derived during world generation from a combination of ClimateBiome, available
/// FloralResources, and hydrological conditions, and stored as a collection of abundance
/// and quality values on CellResourceData.
///
/// Resources represent wild animal populations in the pre-human world state. Domestication
/// of LivestockAnimals, DraftAnimals, and WoolAnimals is a civilization level decision
/// made during simulation, not a world generation output.
///
/// Predator populations are tracked as a resource due to their ecological significance.
/// Depletion of predator populations triggers trophic cascade effects including prey
/// overpopulation, overgrazing, and subsequent vegetation and soil degradation.
///
/// Silkworm availability is derived from ClimateBiome rather than a specific host
/// plant dependency, representing an abstraction of the temperate to subtropical
/// conditions required for silk production.
/// </summary>
public enum FaunaResource
{
    LargeGame,
    SmallGame,
    Bees,
    Fowl,
    Fish,
    MarineMammals,
    WoolAnimals,
    Silkworms,
    LivestockAnimals,
    DraftAnimals,
    Predators
}
