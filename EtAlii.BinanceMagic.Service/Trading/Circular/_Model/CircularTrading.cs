namespace EtAlii.BinanceMagic.Service.Trading.Circular
{
    using System;
    using System.Collections.Generic;

    public class CircularTrading : TradingBase
    {
        public string FirstSymbol { get; set; }
        public string SecondSymbol { get; set; }
        
        public Connectivity Connectivity { get; set; }
        
        public decimal TargetIncrease  { get; init; } = 0.05m; // in %
        public decimal InitialSellFactor { get; init; } = 1.1m; // in %

        public decimal QuantityFactor { get; init; } = 1m;
        public decimal InitialTarget  { get; init; } = 11m; // in BUSD.
        public decimal MaxQuantityToTrade  { get; init; } = 1.0m; // in %.
        public decimal NotionalMinCorrection  { get; init; } = 1.05m; // in %.
        public TimeSpan SampleInterval  { get; init; } = TimeSpan.FromMinutes(1);

        public IList<CircularTradeDetailsSnapshot> Snapshots { get; private set; } = new List<CircularTradeDetailsSnapshot>();
    }
}