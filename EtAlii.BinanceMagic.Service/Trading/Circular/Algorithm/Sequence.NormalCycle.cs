namespace EtAlii.BinanceMagic.Service
{
    using System.Threading;

    public partial class Sequence
    {
        private bool TryHandleNormalCycle(CancellationToken cancellationToken, Situation situation, out bool targetSucceeded)
        {
            var transaction = _context.CurrentTransaction;

            targetSucceeded = false;

            if (_circularAlgorithm.TransactionIsWorthIt(situation, out var sellAction, out var buyAction))
            {
                transaction.Result = $"Preparing to sell {sellAction.Quantity} {sellAction.Symbol} and buy {buyAction.Quantity} {buyAction.Symbol}";
                transaction.LastCheck = _timeManager.GetNow();
                _context.Update(transaction);

                transaction.Result = "Feasible transaction found - Converting...";
                transaction.LastCheck = _timeManager.GetNow();
                _context.Update(transaction);

                if (_client.TryConvert(sellAction, buyAction, _context.Trading.ReferenceSymbol, cancellationToken, _timeManager.GetNow, out var tradeTransaction, out var error))
                {
                    SaveAndReplaceTransaction(transaction, tradeTransaction, false);
                    targetSucceeded = true;
                }
                else
                {
                    transaction.Result = error;
                }
            }

            return targetSucceeded;
        }
    }
}