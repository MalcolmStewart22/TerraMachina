using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Hydrology;

public class HydrologyData
{
    /// <summary> Measured as Meters from the Lowest Elevation on the planet. The lowest elevation should always be the negative copy of thie field</summary>
    public int SeaLevel { get; set; }
    public List<RiverNetwork> RiverNetworks { get; set; } = new();
    public List<Glacier> Glaciers { get; set; } = new();
    public List<Aquifer> Aquifers { get; set; } = new();
}