namespace EtAlii.BinanceMagic
{
    using System.Threading;

    public partial class Loop
    {
        private bool HandleInitialCycle(CancellationToken cancellationToken, Target target, Situation situation)
        {
            var targetSucceeded = false;
            
            Status.Result = "Initial cycle";

            _algorithm.ToInitialConversionActions(target, situation, out var initialSellAction, out var initialBuyAction);
            _status.Result = $"Preparing to sell {initialSellAction.Quantity} {initialSellAction.Coin} and buy {initialBuyAction.Quantity} {initialBuyAction.Coin}";

            if (_client.TryConvert(initialSellAction, initialBuyAction, _settings.ReferenceCoin, _status, cancellationToken, out var transaction))
            {
                transaction = transaction with {TotalProfit = 0m};

                _status.Profit = 0;
                _status.Result = "Transaction done";
                _data.AddTransaction(transaction);
                targetSucceeded = true;
            }

            return targetSucceeded;
        }
    }
}