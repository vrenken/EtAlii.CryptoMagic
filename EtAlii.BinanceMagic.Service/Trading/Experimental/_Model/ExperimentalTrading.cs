namespace EtAlii.BinanceMagic.Service
{
    using System.Collections.Generic;

    public class ExperimentalTrading : TradingBase
    {
        public IList<ExperimentalTradeDetailsSnapshot> Snapshots { get; private set; } = new List<ExperimentalTradeDetailsSnapshot>();
    }
}