namespace EtAlii.CryptoMagic.Service
{
    public record BuyAction : TradeAction;
    public record SellAction : TradeAction;

    public abstract record TradeAction
    {
        public string Symbol { get; init; }
        public decimal Quantity { get; init; }
        public decimal QuotedQuantity { get; init; }
        public decimal Price { get; init; }
        public string TransactionId { get; init; }
    }
}