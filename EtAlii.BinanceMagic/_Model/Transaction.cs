namespace EtAlii.BinanceMagic
{
    using System;

    public record Transaction
    {
        public DateTime Moment { get; init; }
        public Coin From { get; init; }
        public Coin To { get; init; }
        
        public decimal Target { get; init; }
        public decimal Profit { get; init; }
    }
}