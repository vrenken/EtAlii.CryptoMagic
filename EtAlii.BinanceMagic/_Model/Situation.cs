namespace EtAlii.BinanceMagic
{
    using Binance.Net.Objects.Spot.MarketData;

    public record Situation
    {
        public BinanceExchangeInfo ExchangeInfo { get; init; } 
        public decimal SourceSellFee { get; init; }
        public decimal DestinationBuyFee { get; init; }
        
        public Delta Source { get; init; }
        public Delta Destination { get; init; }
        
        public bool IsInitialCycle { get; init; }
    }
}