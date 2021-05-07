namespace EtAlii.BinanceMagic.Surfing
{
    using System;

    public class AlgorithmSettings : IAlgorithmSettings  
    {
        public string FileFormat { get; init; }  = "Transactions_Surfing_{0}.txt";
        public string[] AllowedCoins { get; init; }  = { "BTC", "BNB", "ETH", "LTC", "XMR", "ADA", "RUNE" }; // "ETH"
        public string PayoutCoin  { get; init; } = "USDT";
        public decimal InitialPurchase { get; init; }
        public TimeSpan ActionInterval  { get; init; } = TimeSpan.FromMinutes(1);
    }
}