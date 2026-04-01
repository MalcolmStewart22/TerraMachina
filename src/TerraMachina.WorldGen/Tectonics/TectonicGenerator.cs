using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.WorldGen.Tectonics;


//Build TectonicCellMap
//Spawn seed Plumes
//Build Starting Ridges

//Grow Plates until full
//Asign Base Elevation Noise and Ridge Heights

//Simulate tectonic activity
public class TectonicGenerator
{
    private List<TectonicCell> Cells { get; init; } = new();
    private List<MantlePlume> IsolatedPlumes { get; init; } = new();
    private List<MantlePlume> RidgePlumes { get; init; } = new();
    private List<MantleRidge> Ridges { get; init; } = new();
    private List<TectonicPlate> Plates { get; init; } = new();

}
