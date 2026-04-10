using System;
using System.Collections.Generic;
using System.Text;
using TerraMachina.Abstractions.ProgressUpdate.WorldGen.Enums;

namespace TerraMachina.Abstractions.ProgressUpdate.WorldGen;

public class WorldGenProgressUpdate
{
    public WorldGenStageTypes CurrentStage { get; set; }
    public WorldGenStepTypes? CurrentStep { get; set; }
    public float StageProgress { get; set; }
    public IWorldGenPayload? Payload { get; set; }
    public string? Message { get; set; }
}
