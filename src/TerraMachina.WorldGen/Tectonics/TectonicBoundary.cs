using System;
using System.Collections.Generic;
using System.Text;
using TerraMachina.Abstractions.Enums;

namespace TerraMachina.WorldGen.Tectonics;

public class TectonicBoundary
{
    public int ParentPlateA {  get; set; }
    public int ParentPlateB { get; set; }
    public List<int> PlateACellIDs { get; set; } = new();
    public List<int> PlateBCellIDs { get; set; } = new();
    public BoundaryType BoundaryType { get; set; }
    public ConvergentType? ConvergentAngle { get; init; }
}
