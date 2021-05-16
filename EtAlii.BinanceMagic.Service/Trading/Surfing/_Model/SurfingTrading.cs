namespace EtAlii.BinanceMagic.Service.Trading.Surfing
{
    using System.Collections.Generic;

    public class SurfingTrading : TradingBase
    {
        public IList<SurfingTradeDetailsSnapshot> Snapshots { get; private set; } = new List<SurfingTradeDetailsSnapshot>();
    }
}