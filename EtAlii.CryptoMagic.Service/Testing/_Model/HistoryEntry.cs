namespace EtAlii.CryptoMagic.Service
{
    using System;

    public class HistoryEntry
    {
        public DateTime From { get; init; }
        public DateTime To { get; init; }
        public decimal Open { get; init;}
        public decimal High { get; init; }
        public decimal Low { get; init; }
        public decimal Close { get; init; }
        public decimal Volume { get; init; }
    }
}