namespace EtAlii.BinanceMagic
{
    using System;

    public class StatusInfo
    {
        public string FromCoin { get; set; }
        public string ToCoin { get; set; }
        public string ReferenceCoin { get; set; }
        
        public decimal SellPrice { get; set; }
        public decimal SellQuantity { get; set; }
        public decimal Difference { get; set; }
        public decimal Target { get; set; }

        public DateTime LastSuccess { get; set; }
        public DateTime LastCheck { get; set; }
        public DateTime NextCheck { get; set; }
        
        public decimal Profit { get; set; }
        public string Result { get; set; }
        public bool Error { get; set; }
    }
}