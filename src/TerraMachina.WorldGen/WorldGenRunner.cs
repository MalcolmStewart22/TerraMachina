using TerraMachina.Abstractions.Circulation;
using TerraMachina.Abstractions.Geology;
using TerraMachina.Abstractions.Hydrology;
using TerraMachina.Abstractions.Parameters;
using TerraMachina.Abstractions.ProgressUpdate;
using TerraMachina.Abstractions.ProgressUpdate.WorldGen;
using TerraMachina.Abstractions.Surface;
using TerraMachina.Abstractions.World;
using TerraMachina.WorldGen.Geometry;

namespace TerraMachina.WorldGen;

public class WorldGenRunner
{

    public async Task<World> RunAsync(WorldGenParameters parameters, IProgress<WorldGenProgressUpdate> progress)
    {
        var newWorld = new World
            (
                new CellMap(), 
                new GeologyData(), 
                new HydrologyData(), 
                new CirculationSystems()
            );

        IcoSphereGenerator icosphereGen = new();
        icosphereGen.GenerateSphere(newWorld.Surface, progress, parameters.SubdivisionLevels);


        return newWorld;
    }

}
