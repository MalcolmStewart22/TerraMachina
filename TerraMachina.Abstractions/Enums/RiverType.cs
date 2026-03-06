using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Enums;
/// <summary>
/// Classifies a river by flow permanence based on standard hydrological regime classification.
/// Permanence is determined by the nature of the river's primary feed source
/// rather than its size or position in the network.
/// </summary>
public enum RiverType
{
    Perennial,
    Seasonal,
    Ephemeral
}
