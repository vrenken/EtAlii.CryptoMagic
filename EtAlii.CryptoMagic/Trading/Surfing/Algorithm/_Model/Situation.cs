namespace EtAlii.CryptoMagic.Surfing
{
    using Binance.Net.Objects.Models.Spot;

    public record Situation
    {
        public BinanceExchangeInfo ExchangeInfo { get; init; } 
        public Trend[] Trends { get; init; }
        public string CurrentCoin { get; init; }
        public bool IsInitialCycle { get; init; }
    }
}