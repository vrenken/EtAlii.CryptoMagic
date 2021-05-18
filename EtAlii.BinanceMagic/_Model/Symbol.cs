namespace EtAlii.BinanceMagic
{
    public record Symbol
    {
        public string SymbolName { get; init; }
        public decimal Quantity { get; init; }
        public decimal Price { get; init; }
        public decimal QuoteQuantity { get; init; }
    }
}