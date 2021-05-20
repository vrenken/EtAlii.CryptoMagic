namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Collections.Generic;

    public class CircularTrading : TradingBase
    {
        public string FirstSymbol { get; set; }
        public string SecondSymbol { get; set; }

        public string ReferenceSymbol { get; init; }
        public Connectivity Connectivity { get; set; } = Connectivity.Test;
        
        public decimal TargetIncrease { get; set; } = 1.05m; // in %
        public decimal InitialSellFactor { get; set; } = 1.1m; // in %

        public decimal QuantityFactor { get; set; } = 1m;
        public decimal InitialTarget  { get; set; } = 0.25m; // in reference symbol (USDT).
        public decimal MaxQuantityToTrade  { get; set; } = 1.0m; // in %.
        public decimal NotionalMinCorrection  { get; set; } = 1.05m; // in %.
        public TimeSpan SampleInterval  { get; set; } = TimeSpan.FromMinutes(1);
        
        public int RsiPeriod { get; init; } = 6;

        public IList<CircularTradeSnapshot> Snapshots { get; private set; } = new List<CircularTradeSnapshot>();
        public IList<CircularTradeSnapshot> Transactions { get; private set; } = new List<CircularTradeSnapshot>();
    }
}