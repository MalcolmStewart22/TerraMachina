using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Hydrology;

public class Aquifer
{
    public int AquiferId { get; init; }
    public List<IAquiferNode> Nodes { get; init; } = new();
}