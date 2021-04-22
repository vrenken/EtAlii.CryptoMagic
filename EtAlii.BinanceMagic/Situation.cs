namespace EtAlii.BinanceMagic
{
    public class Situation
    {
        public decimal SourcePurchaseQuantity { get; init; }
        public decimal SourceSellFee { get; init; }
        public decimal TargetBuyFee { get; init; }
        
        public Delta SourceDelta { get; init; }
        public Delta TargetDelta { get; init; }
    }
}