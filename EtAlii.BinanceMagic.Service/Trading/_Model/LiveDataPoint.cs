namespace EtAlii.BinanceMagic.Service
{
    using System;

    public struct LiveDataPoint
    {
        public DateTime X { get; init; }

        public decimal Y { get; init; }
    }
}