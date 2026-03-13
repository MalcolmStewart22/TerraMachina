using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Hydrology;

public class AquiferReservoir : IAquiferNode
{
    public int NodeId { get; init; }
    public List<int> SurfaceCellsById { get; init; } = new();
    public List<IAquiferNode> ConnectedNodes { get; init; } = new();
    public List<int> SpringsByCellId { get; init; } = new();
    /// <summary> Measured in total KiloLiters</summary>
    public float MaxWaterCapacity { get; set; }
    /// <summary> As a percent of MaxCapacity</summary>
    public float CurrentCapacity { get; set; }
    public float Pressure { get; set; }
}