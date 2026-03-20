using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.ProgressUpdate.WorldGen.Enums;

public enum WorldGenStepTypes
{
    GeneratingIcosahedron,
    Subdividing,
    ProjectingToSphere,
    SimulatingPlateMovement,
    FillingOceans,
    GeneratingAtmosphericCurrents,
    GeneratingOceanicCurrents,
    SimulatingWeatherPatterns,
    AssigningBiomes,
    BuildingHydrology,
    AllocatingResources
}
