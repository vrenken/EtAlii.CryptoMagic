namespace EtAlii.BinanceMagic.Tests
{
    using System;

    public class TestContext
    {
        public Random Random => _random;
        private readonly Random _random = new(Environment.TickCount);
        
        public Settings CreateSettings()
        {
            return new()
            {
                TransactionsFile = $"Transactions_{_random.Next()}.txt",
                IsTest = true,
            };
        }

        public Transaction CreateTransaction(string fromCoin, decimal fromPrice, decimal fromQuantity, string toCoin, decimal toPrice, decimal toQuantity, decimal totalProfit, decimal change)
        {
            return new ()
            {
                From = new CoinSnapshot
                {
                    Coin = fromCoin,
                    Price = fromPrice,
                    Quantity = fromQuantity,
                },
                To = new CoinSnapshot
                {
                    Coin = toCoin,
                    Price = toPrice,
                    Quantity = toQuantity,
                },
                Moment = DateTime.Now,
                TotalProfit = totalProfit,
            };
        }
    }
}