using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Enums;
/// <summary>
/// Classifies the foundational bedrock layer underlying a cell.
/// Bedrock type drives several simulation systems including aquifer volume
/// and recharge rate, standing water and drainage, soil type, resource
/// availability, buildability, river canyon morphology, cave formation,
/// and coastal morphology.
/// 
/// Types were selected based on global prevalence and functional distinction
/// across simulation outputs rather than exhaustive geological cataloguing:
/// - Granite and Basalt cover the two fundamental igneous types, representing
///   continental and oceanic/volcanic crust respectively
/// - Limestone, Sandstone, and Shale cover the most globally significant
///   sedimentary types, each producing distinct aquifer and drainage behavior
/// - Slate is included as the primary metamorphic type due to its prevalence
///   in mountain ranges, which are a significant terrain feature of the simulation
/// </summary>
public enum BedrockType
{
    Granite,
    Basalt,
    Limestone,
    Sandstone,
    Shale,
    Slate
}
