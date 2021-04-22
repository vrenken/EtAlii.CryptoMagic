namespace EtAlii.BinanceMagic
{
    public record Target
    {
        public string SourceCoin { get; init; }
        public string TargetCoin { get; init; }
        
        public decimal MinimalRequiredGain { get; init; }
    }
}