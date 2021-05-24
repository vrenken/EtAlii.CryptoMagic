namespace EtAlii.BinanceMagic.Service
{
    using System.Threading;

    public partial class Sequence
    {
        private bool HandleInitialCycle(CancellationToken cancellationToken, Situation situation, CircularTransaction transaction)
        {
            var targetSucceeded = false;
            
            transaction.Result = "Initial cycle";
            transaction.LastCheck = _timeManager.GetNow();
            _context.Update(transaction);

            _circularAlgorithm.ToInitialConversionActions(situation, transaction, out var initialSellAction, out var initialBuyAction);

            if (initialSellAction == null)
            {
                transaction.Result = $"Preparing to buy {initialBuyAction.Quantity} {initialBuyAction.Symbol}";
                transaction.LastCheck = _timeManager.GetNow();
                _context.Update(transaction);

                if (_client.TryBuy(initialBuyAction, _context.Trading.ReferenceSymbol, cancellationToken, _timeManager.GetNow, out TradeTransaction tradeTransaction, out var error))
                {
                    SaveAndReplaceTransaction(transaction, tradeTransaction, true);
                    targetSucceeded = true;
                }
                else
                {
                    transaction.Result = error;
                }
            }
            else
            {
                transaction.Result = $"Preparing to sell {initialSellAction.Quantity} {initialSellAction.Symbol} and buy {initialBuyAction.Quantity} {initialBuyAction.Symbol}";
                transaction.LastCheck = _timeManager.GetNow();
                _context.Update(transaction);

                if (_client.TryConvert(initialSellAction, initialBuyAction, _context.Trading.ReferenceSymbol, cancellationToken, _timeManager.GetNow, out var tradeTransaction, out var error))
                {
                    SaveAndReplaceTransaction(transaction, tradeTransaction, true);
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