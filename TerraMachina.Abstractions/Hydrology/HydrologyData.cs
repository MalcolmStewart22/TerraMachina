using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Hydrology;

public class HydrologyData
{
    public List<RiverNetwork> RiverNetworks = new();
    public List<Glacier> Glaciers = new();
    public List<Aquifer> Aquifers = new();
}