namespace EtAlii.CryptoMagic.Service
{
    using System;

    public record TradeTransaction
    {
        public DateTime Moment { get; init; }
        public Symbol Sell { get; init; }
        public Symbol Buy { get; init; }
        
        public decimal Target { get; init; }
        public decimal Profit { get; init; }
    }
}