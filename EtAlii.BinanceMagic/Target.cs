namespace EtAlii.BinanceMagic
{
    public class Target
    {
        public string SourceCoin { get; init; }
        public string TargetCoin { get; init; }
        
        public decimal MinimalRequiredWinnings { get; init; }
    }
}