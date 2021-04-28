namespace EtAlii.BinanceMagic
{
    public record Target
    {
        public decimal PreviousProfit { get; init; }
        public decimal Profit { get; init; }
    }
}