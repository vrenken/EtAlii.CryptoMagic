namespace EtAlii.BinanceMagic.Surfing
{
    public record Trend
    {
        public string Symbol { get; init; }
        public decimal Open { get; init; }
        public decimal Close { get; init; }
        public decimal High { get; init; }
        public decimal Low { get; init; }
        public decimal Rsi { get; init; }
        public decimal Price { get; init; }
        
    }
}