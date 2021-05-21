namespace EtAlii.BinanceMagic.Service
{
    public class OneOffTrading : TradingBase
    {
        
        public string Symbol { get; set; }
        public decimal PurchasePrice { get; set; }
        
        public decimal QuoteQuantity { get; set; }
        public decimal Quantity { get; set; }

        public decimal SellPrice { get; set; }
    }
}