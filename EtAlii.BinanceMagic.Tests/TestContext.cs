namespace EtAlii.BinanceMagic.Tests
{
    using System;
    using EtAlii.BinanceMagic.Circular;

    public class TestContext
    {
        public Random Random => _random;
        private readonly Random _random = new(Environment.TickCount);
        
        public ProgramSettings CreateProgramSettings() => new();

        public AlgorithmSettings CreateCircularAlgorithmSettings()
        {
            return new()
            {
                TransactionsFileFormat = $"Transactions_{_random.Next()}.txt",
            };
        }

        public Transaction CreateTransaction(string fromCoin, decimal fromPrice, decimal fromQuantity, string toCoin, decimal toPrice, decimal toQuantity, decimal totalProfit, decimal change)
        {
            return new ()
            {
                From = new Coin
                {
                    Symbol = fromCoin,
                    QuoteQuantity = fromPrice,
                    Quantity = fromQuantity,
                },
                To = new Coin
                {
                    Symbol = toCoin,
                    QuoteQuantity = toPrice,
                    Quantity = toQuantity,
                },
                Moment = DateTime.Now,
                Profit = totalProfit,
            };
        }
    }
}