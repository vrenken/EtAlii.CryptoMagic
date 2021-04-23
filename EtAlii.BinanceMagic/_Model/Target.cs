namespace EtAlii.BinanceMagic
{
    public record Target
    {
        public string Source { get; init; }
        public string Destination { get; init; }
        
        public decimal Profit { get; init; }
    }
}