namespace EtAlii.CryptoMagic
{
    using System.Threading;
    using System.Threading.Tasks;

    public partial class Sequence
    {
        private async Task<(bool success, bool targetSucceeded)> TryHandleNormalCycle(CancellationToken cancellationToken, Situation situation)
        {
            var transaction = _context.CurrentTransaction;

            transaction.IsInitialTransaction = false;

            var targetSucceeded = false;
            var success = false;

            if (_circularAlgorithm.TransactionIsWorthIt(situation, out var sellAction, out var buyAction))
            {
                transaction.Result = $"Preparing to sell {sellAction.Quantity} {sellAction.Symbol} and buy {buyAction.Quantity} {buyAction.Symbol}";
                transaction.LastCheck = _timeManager.GetNow();
                _context.Update(transaction);

                transaction.Result = "Feasible transaction found - Converting...";
                transaction.LastCheck = _timeManager.GetNow();
                _context.Update(transaction);

                TradeTransaction tradeTransaction;
                string error;
                (success, tradeTransaction, error) = await  _client.TryConvert(sellAction, buyAction, _context.Trading.ReferenceSymbol, cancellationToken, _timeManager.GetNow);
                if (success)
                {
                    SaveAndReplaceTransaction(transaction, tradeTransaction, false);
                    targetSucceeded = true;
                }
                else
                {
                    transaction.Result = error;
                }
            }

            return (success, targetSucceeded);
        }
    }
}