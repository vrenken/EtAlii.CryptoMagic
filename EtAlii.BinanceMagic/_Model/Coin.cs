namespace EtAlii.BinanceMagic
{
    public record Coin
    {
        public string Symbol { get; init; }
        public decimal Quantity { get; init; }
        public decimal Price { get; init; }
        public decimal QuoteQuantity { get; init; }
    }
}