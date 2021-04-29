namespace EtAlii.BinanceMagic
{
    using Binance.Net.Objects.Spot.MarketData;

    public record Situation
    {
        public BinanceExchangeInfo ExchangeInfo { get; init; } 
        public decimal SellFee { get; init; }
        public decimal SellTrend { get; init; }
        public decimal BuyFee { get; init; }
        public decimal BuyTrend { get; init; }
        
        public Delta Source { get; init; }
        public Delta Destination { get; init; }
        
        public bool IsInitialCycle { get; init; }
    }
}