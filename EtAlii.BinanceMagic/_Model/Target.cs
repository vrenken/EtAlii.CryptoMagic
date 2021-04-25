namespace EtAlii.BinanceMagic
{
    public record Target
    {
        public string Source { get; init; }
        public string Destination { get; init; }
        
        public decimal PreviousProfit { get; init; }
        public decimal Profit { get; init; }
        public int TransactionId { get; init; }
    }
}