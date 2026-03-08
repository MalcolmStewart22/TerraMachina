using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.World;

public class World
{
    public CellMap Surface = new();
    public GeologyData Geology = new();
    public HydrologyData Hydrology = new();
    public CirculationSystems Circulation = new();
}
