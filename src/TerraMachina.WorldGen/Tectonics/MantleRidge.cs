using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TerraMachina.WorldGen.Tectonics;
/// <summary>
/// Simplification: Ridges are treated as fixed boundaries throughout the tectonic simulation.
/// Ridge push forces act on plate cells rather than advancing the ridge boundary outward.
/// In reality, ridges migrate outward as new crust is created at the boundary, causing plates
/// to grow over time. This is most visible in the Antarctic plate, which is surrounded by
/// divergent boundaries on all sides and grows outward rather than translating across the mantle.
/// This behaviour has been omitted as the primary output of the tectonic phase is terrain variety
/// and boundary interactions rather than accurate plate growth over time. The effect is most
/// pronounced in edge cases where a plate is surrounded by ridges on all sides, which is
/// unlikely to occur frequently in normal generation. This simplification can be revisited
/// post-implementation if greater geological accuracy is desired.
/// </summary>
public class MantleRidge
{
    static int NextID = 1;
    public int RidgeId { get; init; }
    public int PlumeAId { get; init; }
    public int PlumeBId { get; init; }
    public float LateralAngle {  get; init; }
    public Vector3 Normal { get; init;  }
    public List<RidgeSegment> Segments { get; init; } = new();
    public List<int> CenterCellIds { get; init; } = new();
    public List<int> ContainedCellIds { get; init; } = new();

    public MantleRidge(int plumeAID, int plumeBID, float lateralAngle, Vector3 normal, List<RidgeSegment> segments)
    {
        RidgeId = NextID;
        NextID++;
        PlumeAId = plumeAID;
        PlumeBId = plumeBID;
        LateralAngle = lateralAngle;
        Segments = [..segments];
        Normal = normal;
    }
}
