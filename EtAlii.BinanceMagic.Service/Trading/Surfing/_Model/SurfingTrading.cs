namespace EtAlii.BinanceMagic.Service
{
    using System.Collections.Generic;

    public class SurfingTrading : TradingBase
    {
        public IList<SurfingTradeDetailsSnapshot> Snapshots { get; private set; } = new List<SurfingTradeDetailsSnapshot>();
    }
}