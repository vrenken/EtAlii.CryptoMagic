namespace EtAlii.BinanceMagic.Service
{
    using System.Collections.Generic;

    public class SimpleTrading : Trading
    {
        public IList<SimpleTradeDetailsSnapshot> Snapshots { get; private set; } = new List<SimpleTradeDetailsSnapshot>();
    }
}