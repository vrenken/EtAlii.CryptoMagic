﻿namespace EtAlii.BinanceMagic
{
    using System;

    public record Transaction
    {
        public DateTime Moment { get; init; }
        public Symbol Sell { get; init; }
        public Symbol Buy { get; init; }
        
        public decimal Target { get; init; }
        public decimal Profit { get; init; }
    }
}