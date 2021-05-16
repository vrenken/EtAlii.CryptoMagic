namespace EtAlii.BinanceMagic.Service
{
    using System.Collections.Generic;

    public class SurfingTrading : Trading
    {
        public IList<SurfingTradeDetailsSnapshot> Snapshots { get; private set; } = new List<SurfingTradeDetailsSnapshot>();
    }
}