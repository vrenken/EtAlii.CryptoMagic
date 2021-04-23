namespace EtAlii.BinanceMagic
{
    public record Situation
    {
        public decimal SourceSellFee { get; init; }
        public decimal DestinationBuyFee { get; init; }
        
        public Delta SourceDelta { get; init; }
        public Delta DestinationDelta { get; init; }
        
        public bool IsInitialCycle { get; init; }
    }
}