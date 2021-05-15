namespace EtAlii.BinanceMagic.Circular
{
    using System;
    using System.Linq;

    public class TradeDetailsUpdater : ITradeDetailsBuilder
    {
        private readonly IData _data;
        private readonly AlgorithmSettings _settings;

        public TradeDetailsUpdater(IData data, AlgorithmSettings settings)
        {
            _data = data;
            _settings = settings;
        }

        public void UpdateTargetDetails(TradeDetails details)
        {
            var lastTransaction = _data.History.LastOrDefault();

            var source = lastTransaction == null
                ? _settings.AllowedCoins.First()
                : lastTransaction.BuyCoin;
            var destination = lastTransaction == null
                ? _settings.AllowedCoins.Skip(1).First()
                : lastTransaction.SellCoin;

            var target = lastTransaction != null
                ? lastTransaction.Target * _settings.TargetIncrease
                : _settings.InitialTarget;
            
            details.LastSuccess = lastTransaction?.NextCheck ?? DateTime.MinValue;
            details.Profit = _data.GetTotalProfits();

            details.SellCoin = source;
            details.BuyCoin = destination;
            details.ReferenceCoin = _settings.ReferenceCoin;
            details.Step = _data.History.Count + 1;
            details.Target = target;
            
            details.Result = "Found next target";
        }
    }
}