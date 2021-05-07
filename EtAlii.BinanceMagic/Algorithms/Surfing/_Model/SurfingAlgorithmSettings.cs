namespace EtAlii.BinanceMagic.Surfing
{
    using System;

    public class SurfingAlgorithmSettings : IAlgorithmSettings  
    {
        public string Transactions FileFormat { get; init; }  = "Transactions_Surfing_{0}_{1}.txt";
        public string TrendsFileFormat { get; init; }  = "Trends_Surfing_{0}_{1}.txt";
        public string[] AllowedCoins { get; init; }  = { "BTC", "BNB" }; // "ETH"
        public string ReferenceCoin  { get; init; } = "BUSD";
        public TimeSpan ActionInterval  { get; init; } = TimeSpan.FromMinutes(1);
    }
}