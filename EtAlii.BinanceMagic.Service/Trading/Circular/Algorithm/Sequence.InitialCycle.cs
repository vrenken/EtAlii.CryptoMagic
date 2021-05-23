namespace EtAlii.BinanceMagic.Service
{
    using System.Threading;

    public partial class Sequence
    {
        private bool HandleInitialCycle(CancellationToken cancellationToken, Situation situation)
        {
            var transaction = _context.CurrentTransaction;
            
            var targetSucceeded = false;
            
            transaction.Result = "Initial cycle";
            transaction.LastCheck = _timeManager.GetNow();
            _context.Update(transaction);

            _circularAlgorithm.ToInitialConversionActions(situation, out var initialSellAction, out var initialBuyAction);
            transaction.Result = $"Preparing to sell {initialSellAction.Quantity} {initialSellAction.Symbol} and buy {initialBuyAction.Quantity} {initialBuyAction.Symbol}";
            transaction.LastCheck = _timeManager.GetNow();
            _context.Update(transaction);

            if (_client.TryConvert(initialSellAction, initialBuyAction, _context.Trading.ReferenceSymbol, cancellationToken, _timeManager.GetNow, out var tradeTransaction, out var error))
            {
                SaveAndReplaceTransaction(transaction, tradeTransaction);
                targetSucceeded = true;
            }
            else
            {
                transaction.Result = error;
            }
            return targetSucceeded;
        }
    }
}