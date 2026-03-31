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
using TerraMachina.WorldGen.Tectonics;

namespace TerraMachina.WorldGen;

public class WorldGenRunner
{
    private World currentWorld;
    private IProgress<WorldGenProgressUpdate> currentProgress;
    private WorldGenParameters currentParameters;
    public WorldGenRunner(WorldGenParameters parameters, IProgress<WorldGenProgressUpdate> progress, World world)
    {
        currentWorld = world;
        currentProgress = progress;
        currentParameters = parameters;
    }
    public async Task RunGeometryAsync()
    {
        currentProgress.Report(new WorldGenProgressUpdate
        {
            CurrentStage = WorldGenStageTypes.Starting,
            StageProgress = 20 * (float)Math.Pow(4, currentParameters.SubdivisionLevels),
        });
        IcoSphereGenerator icosphereGen = new();
        icosphereGen.GenerateSphere(currentWorld.Surface, currentProgress, currentParameters.SubdivisionLevels);
    }

    public async Task RunTectonicsAsync()
    {
        TectonicGenerator tectonicGen = new();
        tectonicGen.G
    }

}
