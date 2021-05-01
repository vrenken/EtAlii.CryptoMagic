namespace EtAlii.BinanceMagic
{
    using System;

    public class CircularAlgorithmSettings : IAlgorithmSettings  
    {
        public string TransactionsFileFormat { get; init; }  = "Transactions_{0}_{1}.txt";
        public string TrendsFileFormat { get; init; }  = "Trends_{0}_{1}.txt";
        public string[] AllowedCoins { get; init; }  = { "BTC", "BNB" }; // "ETH"
        public string ReferenceCoin  { get; init; } = "BUSD";
        public decimal TargetIncrease  { get; init; } = 0.05m; // in %
        public decimal InitialSellFactor { get; init; } = 1.1m; // in %

        public decimal QuantityFactor { get; init; } = 1m;
        public decimal InitialTarget  { get; init; } = 11m; // in BUSD.
        public decimal MaxQuantityToTrade  { get; init; } = 1.0m; // in %.
        public decimal NotionalMinCorrection  { get; init; } = 1.05m; // in %.
        public TimeSpan SampleInterval  { get; init; } = TimeSpan.FromMinutes(1);

        public bool WriteTrends { get; init; } = true;
    }
}