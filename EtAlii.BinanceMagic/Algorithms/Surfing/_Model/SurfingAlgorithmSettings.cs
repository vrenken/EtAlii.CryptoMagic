namespace EtAlii.BinanceMagic.Surfing
{
    using System;

    public class AlgorithmSettings : IAlgorithmSettings  
    {
        public string FileFormat { get; init; }  = "Transactions_Surfing_{0}_{1}.txt";
        public string TrendsFileFormat { get; init; }  = "Trends_Surfing_{0}_{1}.txt";
        public string[] AllowedCoins { get; init; }  = { "BTC", "BNB", "ETH", "LTC", "XMR", "ADA", "RUNE" }; // "ETH"
        public string PayoutCoin  { get; init; } = "USDT";
        public TimeSpan ActionInterval  { get; init; } = TimeSpan.FromMinutes(1);
    }
}