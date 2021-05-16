namespace EtAlii.BinanceMagic.Service.Trading.Simple
{
    using System.Collections.Generic;
    using EtAlii.BinanceMagic.Service.Trading;

    public class SimpleTrading : TradingBase
    {
        public IList<SimpleTradeDetailsSnapshot> Snapshots { get; private set; } = new List<SimpleTradeDetailsSnapshot>();
    }
}