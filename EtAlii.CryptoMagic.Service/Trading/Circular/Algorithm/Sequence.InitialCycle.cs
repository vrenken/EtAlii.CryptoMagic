namespace EtAlii.CryptoMagic.Service
{
    using System.Threading;
    using System.Threading.Tasks;

    public partial class Sequence
    {
        private async Task<bool> HandleInitialCycle(CancellationToken cancellationToken, Situation situation, CircularTransaction transaction)
        {
            var targetSucceeded = false;

            transaction.IsInitialTransaction = true;
            transaction.Result = "Initial cycle";
            transaction.LastCheck = _timeManager.GetNow();
            _context.Update(transaction);

            _circularAlgorithm.ToInitialConversionActions(situation, transaction, out var initialSellAction, out var initialBuyAction);

            if (initialSellAction == null)
            {
                transaction.Result = $"Preparing to buy {initialBuyAction.Quantity} {initialBuyAction.Symbol}";
                transaction.LastCheck = _timeManager.GetNow();
                _context.Update(transaction);

                TradeTransaction tradeTransaction;
                bool success;
                string error;
                (success, tradeTransaction, error) = await _client.TryBuyTransaction(initialBuyAction, _context.Trading.ReferenceSymbol, cancellationToken, _timeManager.GetNow);
                if (success)
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

                var (success, tradeTransaction, error) = await _client.TryConvert(initialSellAction, initialBuyAction, _context.Trading.ReferenceSymbol, cancellationToken, _timeManager.GetNow);
                if (success)
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