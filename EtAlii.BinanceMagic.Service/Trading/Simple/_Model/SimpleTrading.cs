namespace EtAlii.BinanceMagic.Service
{
    using System.Collections.Generic;

    public class SimpleTrading : TradingBase
    {
        public IList<SimpleTransaction> Transactions { get; private set; } = new List<SimpleTransaction>();
    }
}