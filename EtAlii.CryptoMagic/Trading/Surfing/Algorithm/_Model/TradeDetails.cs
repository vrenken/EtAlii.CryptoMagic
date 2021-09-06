namespace EtAlii.CryptoMagic.Surfing
{
    using System;

    public class TradeDetails
    {
        public string CurrentSymbol { get; set; }
        public decimal CurrentVolume { get; set; }
        public string PayoutSymbol { get; set; }
        public decimal[] Owned { get; set; }
        public Trend[] Trends { get; set; } = Array.Empty<Trend>();
        
        public DateTime LastSuccess { get; set; }
        public DateTime NextCheck { get; set; }
        
        public decimal LastProfit { get; set; }
        public decimal TotalProfit { get; set; }
        public string Status { get; set; }
        public int Step { get; set; }
        
    }
}