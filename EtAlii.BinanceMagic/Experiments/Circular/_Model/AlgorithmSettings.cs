namespace EtAlii.BinanceMagic.Circular
{
    using System;

    public class AlgorithmSettings : IAlgorithmSettings  
    {
        public string TransactionsFileFormat { get; init; }  = "Transactions_{0}_{1}.txt";
        public string TrendsFileFormat { get; init; }  = "Trends_{0}_{1}.txt";
        public string[] AllowedCoins { get; init; }  = { "BTC", "BNB" }; // "ETH"
        public string ReferenceCoin  { get; init; } = "BUSD";
        public decimal TargetIncrease  { get; init; } = 1.03m; // in %
        public decimal InitialSellFactor { get; init; } = 1.1m; // in %

        public decimal QuantityFactor { get; init; } = 10m;
        public decimal InitialTarget  { get; init; } = 0.5m; // in BUSD.
        public decimal MaxQuantityToTrade  { get; init; } = 1.0m; // in %.
        public decimal NotionalMinCorrection  { get; init; } = 1.05m; // in %.
        public TimeSpan SampleInterval  { get; init; } = TimeSpan.FromMinutes(1);
    }
}