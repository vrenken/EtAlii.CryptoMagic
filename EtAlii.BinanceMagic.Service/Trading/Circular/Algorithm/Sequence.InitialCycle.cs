namespace EtAlii.BinanceMagic.Service
{
    using System.Threading;

    public partial class Sequence
    {
        private bool HandleInitialCycle(CancellationToken cancellationToken, Situation situation)
        {
            var snapshot = _context.Snapshot;
            
            var targetSucceeded = false;
            
            snapshot.Result = "Initial cycle";
            snapshot.LastCheck = _timeManager.GetNow();
            _context.RaiseChanged();

            _circularAlgorithm.ToInitialConversionActions(situation, out var initialSellAction, out var initialBuyAction);
            snapshot.Result = $"Preparing to sell {initialSellAction.Quantity} {initialSellAction.Symbol} and buy {initialBuyAction.Quantity} {initialBuyAction.Symbol}";
            snapshot.LastCheck = _timeManager.GetNow();
            _context.RaiseChanged();

            if (_client.TryConvert(initialSellAction, initialBuyAction, _context.Trading.ReferenceSymbol, cancellationToken, _timeManager.GetNow, out var transaction, out var error))
            {
                SaveAndReplaceSnapshot(snapshot, transaction);
                targetSucceeded = true;
            }
            else
            {
                snapshot.Result = error;
            }
            return targetSucceeded;
        }
    }
}