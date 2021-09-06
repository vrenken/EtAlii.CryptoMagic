namespace EtAlii.CryptoMagic.Service
{
    using System;
    using System.Collections.Generic;

    public class EdgeTrading : TradingBase
    {
       
        public string Symbol { get; set; }
        public TradeMethod TradeMethod { get; set; } = TradeMethod.BinanceTest;
        public decimal InitialPurchase { get; set; }
        public DateTime Purchased { get; set; }
        
        
        // public decimal QuoteQuantity { get; set; }
        // public decimal Quantity { get; set; }
        //
        // public decimal SellPrice { get; set; }
        
        public IList<EdgeTransaction> Transactions { get; private set; } = new List<EdgeTransaction>();
    }
}