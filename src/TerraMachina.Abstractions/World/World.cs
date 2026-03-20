using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.World;

public class World
{
    public CellMap Surface { get; set; }
    public GeologyData Geology { get; set; }
    public HydrologyData Hydrology { get; set; }
    public CirculationSystems Circulation { get; set; }

    public World (CellMap surface, GeologyData geology, HydrologyData hydrology, CirculationSystems circulation)
    {
        Surface = surface;
        Geology = geology;
        Hydrology = hydrology;
        Circulation = circulation;
    }
}
