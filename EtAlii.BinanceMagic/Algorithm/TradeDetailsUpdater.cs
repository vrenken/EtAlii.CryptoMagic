namespace EtAlii.BinanceMagic
{
    using System;
    using System.Linq;

    public class TradeDetailsUpdater : ITradeDetailsBuilder
    {
        private readonly IData _data;
        private readonly LoopSettings _settings;

        public TradeDetailsUpdater(IData data, LoopSettings settings)
        {
            _data = data;
            _settings = settings;
        }

        public void UpdateTargetDetails(TradeDetails details)
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

            details.LastSuccess = lastTransaction?.Moment ?? DateTime.MinValue;
            details.Profit = previousProfit;

            details.SellCoin = source;
            details.BuyCoin = destination;
            details.TransactionId = _data.Transactions.Count + 1;
            details.ReferenceCoin = _settings.ReferenceCoin;

            details.PreviousProfit = previousProfit;
            details.Goal = profit;
            
            details.Result = "Found next target";
        }
    }
}