using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.ProgressUpdate.WorldGen.Tectonics;

public class TectonicCellMinimum
{
    public int CellID { get; init; }
    public float Elevation { get; set; }
    public int? PlateID { get; init; }

    public TectonicCellMinimum(int id, float elevation, int? plateID)
    {
        CellID = id;
        Elevation = elevation;
        PlateID = plateID;
    }
}
