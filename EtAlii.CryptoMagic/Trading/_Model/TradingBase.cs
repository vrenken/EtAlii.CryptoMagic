namespace EtAlii.CryptoMagic
{
    using System;

    public class TradingBase : Entity
    {
        public string Name { get; set; }
        public string ReferenceSymbol { get; init; }
        
        public decimal TotalProfit { get; set; }
        
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
    }
}