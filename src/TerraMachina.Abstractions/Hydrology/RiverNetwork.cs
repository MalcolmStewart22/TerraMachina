using System;
using System.Collections.Generic;
using System.Text;

namespace TerraMachina.Abstractions.Hydrology;

public class RiverNetwork
{
    public int RiverNetworkId { get; init; }
    public River MainRiverId { get; set; }
    public List<River> ConnectedRivers { get; set; }
    public RiverStatusType RiverNetworkStatus { get; set; }
}