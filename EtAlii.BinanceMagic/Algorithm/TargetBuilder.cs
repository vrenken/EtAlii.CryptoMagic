namespace EtAlii.BinanceMagic
{
    using System.Linq;

    public class TargetBuilder : ITargetBuilder
    {
        private readonly IData _data;
        private readonly LoopSettings _settings;

        public TargetBuilder(IData data, LoopSettings settings)
        {
            _data = data;
            _settings = settings;
        }

        public Target BuildTarget()
        {
            var lastTransaction = _data.Transactions.LastOrDefault();

            var source = lastTransaction == null
                ? _settings.AllowedCoins.First()
                : lastTransaction.To.Coin;
            var destination = lastTransaction == null
                ? _settings.AllowedCoins.Skip(1).First()
                : lastTransaction.From.Coin;

            var profit = lastTransaction != null
                ? lastTransaction.TotalProfit * (1 + _settings.MinimalIncrease)
                : _settings.MinimalTargetProfit;

            profit = profit < _settings.MinimalTargetProfit 
                ? _settings.MinimalTargetProfit 
                : profit;

            var previousProfit = lastTransaction?.TotalProfit ?? profit;
            previousProfit = previousProfit > 0 ? previousProfit : profit; 

            return new Target
            {
                Source = source,
                Destination = destination,
                PreviousProfit = previousProfit,
                Profit = profit,
                TransactionId = _data.Transactions.Count + 1,
            };
        }

    }
}