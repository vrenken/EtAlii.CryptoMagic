namespace EtAlii.BinanceMagic
{
    public record BuyAction : Action;
    public record SellAction : Action;

    public abstract record Action
    {
        public string Symbol { get; init; }
        public decimal Quantity { get; init; }
        public decimal QuotedQuantity { get; init; }
        public decimal Price { get; init; }
        public string TransactionId { get; init; }
    }
}