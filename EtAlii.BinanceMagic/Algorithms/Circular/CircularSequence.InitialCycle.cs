namespace EtAlii.BinanceMagic
{
    using System.Threading;

    public partial class CircularSequence
    {
        private bool HandleInitialCycle(CancellationToken cancellationToken, Situation situation)
        {
            var targetSucceeded = false;
            
            _details.Result = "Initial cycle";

            _circularAlgorithm.ToInitialConversionActions(situation, out var initialSellAction, out var initialBuyAction);
            _details.Result = $"Preparing to sell {initialSellAction.Quantity} {initialSellAction.Coin} and buy {initialBuyAction.Quantity} {initialBuyAction.Coin}";

            if (_client.TryConvert(initialSellAction, initialBuyAction, _settings.ReferenceCoin, _details, cancellationToken, out var transaction))
            {
                transaction = transaction with {TotalProfit = 0m};

                _details.Profit = 0;
                _details.Result = "Transaction done";
                _data.AddTransaction(transaction);
                targetSucceeded = true;
            }

            return targetSucceeded;
        }
    }
}