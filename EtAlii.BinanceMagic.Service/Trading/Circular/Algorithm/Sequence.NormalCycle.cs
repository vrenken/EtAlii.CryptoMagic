namespace EtAlii.BinanceMagic.Service
{
    using System.Threading;

    public partial class Sequence
    {
        private bool TryHandleNormalCycle(CancellationToken cancellationToken, Situation situation, out bool targetSucceeded)
        {
            var snapshot = _context.Snapshot;

            targetSucceeded = false;

            if (_circularAlgorithm.TransactionIsWorthIt(situation, out var sellAction, out var buyAction))
            {
                snapshot.Result = $"Preparing to sell {sellAction.Quantity} {sellAction.Symbol} and buy {buyAction.Quantity} {buyAction.Symbol}";
                snapshot.LastCheck = _timeManager.GetNow();
                _context.RaiseChanged();

                snapshot.Result = "Feasible transaction found - Converting...";
                snapshot.LastCheck = _timeManager.GetNow();
                _context.RaiseChanged();

                if (_client.TryConvert(sellAction, buyAction, _context.Trading.ReferenceSymbol, cancellationToken, _timeManager.GetNow, out var transaction, out var error))
                {
                    SaveAndReplaceSnapshot(snapshot, transaction);
                    targetSucceeded = true;
                }
                else
                {
                    snapshot.Result = error;
                }
            }

            return targetSucceeded;
        }
    }
}