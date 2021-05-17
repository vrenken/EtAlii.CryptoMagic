namespace EtAlii.BinanceMagic.Service.Trading.Circular
{
    using System.Collections.Generic;

    public class CircularTrading : TradingBase
    {
        public string FirstSymbol { get; set; }
        public string SecondSymbol { get; set; }
        
        public Connectivity Connectivity { get; set; }
        public IList<CircularTradeDetailsSnapshot> Snapshots { get; private set; } = new List<CircularTradeDetailsSnapshot>();
    }
}