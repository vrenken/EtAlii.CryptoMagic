namespace EtAlii.BinanceMagic.Surfing
{
    using System;

    public class TradeDetails
    {
        public string CurrentCoin { get; set; }
        public string PayoutCoin { get; set; }
        public decimal[] Owned { get; set; } 
        public Trend[] Trends { get; set; }
        
        public DateTime LastSuccess { get; set; }
        public DateTime NextCheck { get; set; }
        
        public decimal LastProfit { get; set; }
        public decimal TotalProfit { get; set; }
        public string Status { get; set; }
        public int Step { get; set; }
        
    }
}