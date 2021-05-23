namespace EtAlii.BinanceMagic.Service
{
    public class TradingBase : Entity
    {
        public string Name { get; set; }
        public string ReferenceSymbol { get; init; }
        
        public decimal TotalProfit { get; set; }
    }
}