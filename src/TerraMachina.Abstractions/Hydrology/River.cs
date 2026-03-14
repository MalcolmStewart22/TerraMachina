using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Hydrology;

public class River
{
    public int RiverId {  get; init; }
    public RiverStatusType RiverStatus { get; set; }
    public required List<IRiverNode> Nodes { get; set; }
    public RiverType RiverType { get; set; }
    public required IRiverNode Head { get; set; }
    public required IRiverNode Mouth { get; set; }
    public int TributaryOfRiverId { get; set; }
}