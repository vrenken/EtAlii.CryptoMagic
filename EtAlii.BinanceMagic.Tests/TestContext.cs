namespace EtAlii.BinanceMagic.Tests
{
    using System;

    public class TestContext
    {
        public Random Random => _random;
        private readonly Random _random = new(Environment.TickCount);
    }
}