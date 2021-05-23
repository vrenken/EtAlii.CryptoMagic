namespace EtAlii.BinanceMagic.Service.Surfing
{
    using Binance.Net.Objects.Spot.MarketData;

    public record Situation
    {
        public BinanceExchangeInfo ExchangeInfo { get; init; } 
        public Trend[] Trends { get; init; }
        public string CurrentCoin { get; init; }
        public bool IsInitialCycle { get; init; }
    }
}