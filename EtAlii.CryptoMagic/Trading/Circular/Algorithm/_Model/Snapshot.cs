namespace EtAlii.CryptoMagic
{
    using System;

    public class Snapshot : Entity
    {
        public CircularTrading Trading { get; set; }
        public DateTime Moment { get; set; }
        public decimal FirstSymbolMarketPrice { get; set; } 
        public decimal SecondSymbolMarketPrice { get; set; } 
    }
}