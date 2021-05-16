namespace EtAlii.BinanceMagic.Service
{
    using System.Collections.Generic;

    public class CircularTrading : TradingBase
    {
        public IList<CircularTradeDetailsSnapshot> Snapshots { get; private set; } = new List<CircularTradeDetailsSnapshot>();
    }
}