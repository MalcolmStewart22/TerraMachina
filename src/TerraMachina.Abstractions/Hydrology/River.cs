using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Hydrology;

public class River
{
    public int RiverId {  get; init; }
    public RiverStatusType RiverStatus { get; set; }
    public List<IRiverNode> Nodes { get; set; }
    public RiverType RiverType { get; set; }
    public IRiverNode Head { get; set; }
    public IRiverNode Mouth { get; set; }
    public int TributaryOfRiverId { get; set; }
}