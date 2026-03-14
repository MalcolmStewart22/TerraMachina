using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.World;

public class World
{
    public required CellMap Surface { get; set; }
    public required GeologyData Geology { get; set; }
    public required HydrologyData Hydrology { get; set; }
    public required CirculationSystems Circulation { get; set; }
}
