namespace EtAlii.BinanceMagic
{
    public record BuyAction : Action;
    public record SellAction : Action;

    public abstract record Action
    {
        public string Coin { get; init; }
        public decimal Quantity { get; init; }
        public decimal TargetPrice { get; init; }
        public string TransactionId { get; init; }
    }
}