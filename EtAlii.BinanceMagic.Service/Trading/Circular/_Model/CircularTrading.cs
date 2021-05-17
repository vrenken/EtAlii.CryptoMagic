namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Collections.Generic;

    public class CircularTrading : TradingBase
    {
        public string FirstSymbol { get; set; }
        public string SecondSymbol { get; set; }

        public Connectivity Connectivity { get; set; } = Connectivity.Test;
        
        public decimal TargetIncrease { get; set; } = 0.05m; // in %
        public decimal InitialSellFactor { get; set; } = 1.1m; // in %

        public decimal QuantityFactor { get; set; } = 1m;
        public decimal InitialTarget  { get; set; } = 11m; // in BUSD.
        public decimal MaxQuantityToTrade  { get; set; } = 1.0m; // in %.
        public decimal NotionalMinCorrection  { get; set; } = 1.05m; // in %.
        public TimeSpan SampleInterval  { get; set; } = TimeSpan.FromMinutes(1);

        public IList<CircularTradeDetailsSnapshot> Snapshots { get; private set; } = new List<CircularTradeDetailsSnapshot>();
    }
}