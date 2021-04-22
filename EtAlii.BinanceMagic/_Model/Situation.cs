namespace EtAlii.BinanceMagic
{
    public record Situation
    {
        public decimal SourceSellFee { get; init; }
        public decimal TargetBuyFee { get; init; }
        
        public Delta SourceDelta { get; init; }
        public Delta TargetDelta { get; init; }
        
        public bool IsInitialCycle { get; init; }
    }
}