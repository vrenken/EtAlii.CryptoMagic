namespace EtAlii.BinanceMagic
{
    public record SellAction
    {
        public string Coin { get; init; }
        public decimal Quantity { get; init; }
        public decimal TargetPrice { get; init; }
        public string TransactionId { get; init; }
    }
}