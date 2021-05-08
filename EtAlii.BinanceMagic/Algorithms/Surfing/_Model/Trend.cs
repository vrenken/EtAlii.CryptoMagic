namespace EtAlii.BinanceMagic.Surfing
{
    public record Trend
    {
        public string Coin { get; init; }
        public decimal Change { get; init; }
        public decimal Price { get; init; }
        
    }
}