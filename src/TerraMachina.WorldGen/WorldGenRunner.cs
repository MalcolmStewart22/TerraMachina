using TerraMachina.Abstractions.Circulation;
using TerraMachina.Abstractions.Geology;
using TerraMachina.Abstractions.Hydrology;
using TerraMachina.Abstractions.Parameters;
using TerraMachina.Abstractions.ProgressUpdate;
using TerraMachina.Abstractions.ProgressUpdate.WorldGen;
using TerraMachina.Abstractions.ProgressUpdate.WorldGen.Enums;
using TerraMachina.Abstractions.Surface;
using TerraMachina.Abstractions.World;
using TerraMachina.WorldGen.Geometry;

namespace TerraMachina.WorldGen;

public class WorldGenRunner
{

    public async Task RunAsync(WorldGenParameters parameters, IProgress<WorldGenProgressUpdate> progress, World world)
    {
        progress.Report(new WorldGenProgressUpdate
        {
            CurrentStage = WorldGenStageTypes.Starting,
            StageProgress = 20 * 4 * parameters.SubdivisionLevels,
        });
        IcoSphereGenerator icosphereGen = new();
        icosphereGen.GenerateSphere(world.Surface, progress, parameters.SubdivisionLevels);
    }

}
