namespace EtAlii.CryptoMagic.Service.Surfing
{
    using System;

    public class AlgorithmSettings  
    {
        public string[] AllowedCoins { get; init; }  = { "BTC", "BNB", "ETH", "LTC", "XMR", "ADA", "RUNE" }; // "ETH"
        public string PayoutCoin  { get; init; } = "USDT";
        public decimal InitialPurchase { get; init; }
        public decimal TransferFactor { get; init; } = 1m;
        public decimal RsiOverSold { get; init; } = 0.70m;
        public decimal RsiOverBought { get; init; } = 0.30m;
         
        public int RsiPeriod { get; init; } = 6;
        public TimeSpan ActionInterval  { get; init; } = TimeSpan.FromMinutes(1);
    }
}