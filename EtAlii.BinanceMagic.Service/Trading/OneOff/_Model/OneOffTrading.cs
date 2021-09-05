namespace EtAlii.BinanceMagic.Service
{
    using System;

    public class OneOffTrading : TradingBase
    {
        
        public string Symbol { get; set; }
        public decimal PurchasePrice { get; set; }
        
        public decimal QuoteQuantity { get; set; }
        public decimal Quantity { get; set; }

        public decimal SellPrice { get; set; }
        
        public TradeMethod TradeMethod { get; set; } = TradeMethod.BinanceTest;
        
        public bool IsCancelled { get; set; }
        public bool IsSuccess { get; set; }
        public decimal Profit { get; set; }
        public decimal Loss { get; set; }
        
        public TimeSpan SampleInterval  { get; set; } = TimeSpan.FromMinutes(2);
    }
}