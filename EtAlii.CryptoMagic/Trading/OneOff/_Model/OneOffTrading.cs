namespace EtAlii.CryptoMagic
{
    using System;

    public class OneOffTrading : TradingBase
    {
        
        public string Symbol { get; set; }

        /// <summary>
        /// Coin quantity (in reference coins).
        /// </summary>
        public decimal PurchaseQuoteQuantity { get; set; } = 50;

        public decimal PurchaseSymbolQuantity { get; set; }

        /// <summary>
        /// Price of one coin at time of purchase (in reference coin).
        /// </summary>
        public decimal PurchasePrice { get; set; }

        /// <summary>
        /// Price of one coin at last check (in reference coin).
        /// </summary>
        public decimal CurrentPrice { get; set; }

        public decimal? SellQuoteQuantity { get; set; }
        public decimal TargetPercentageIncrease { get; set; } = 5;
        public decimal CurrentPercentageIncrease { get; set; }
        
        public TradeMethod TradeMethod { get; set; } = TradeMethod.BinanceTest;
        
        public bool IsCancelled { get; set; }
        public bool IsSuccess { get; set; }
        public decimal FinalQuoteQuantity { get; set; }
        
        public TimeSpan SampleInterval  { get; set; } = TimeSpan.FromMinutes(2);
    }
}