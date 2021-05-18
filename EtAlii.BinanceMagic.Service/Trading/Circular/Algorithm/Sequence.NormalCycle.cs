namespace EtAlii.BinanceMagic.Service
{
    using System.Threading;
    using Microsoft.EntityFrameworkCore;

    public partial class Sequence
    {
        private bool TryHandleNormalCycle(CancellationToken cancellationToken, Situation situation, out bool targetSucceeded)
        {
            var snapshot = _statusProvider.Snapshot;

            targetSucceeded = false;

            if (_circularAlgorithm.TransactionIsWorthIt(situation, out var sellAction, out var buyAction))
            {
                snapshot.Result = $"Preparing to sell {sellAction.Quantity} {sellAction.Symbol} and buy {buyAction.Quantity} {buyAction.Symbol}";
                snapshot.LastCheck = _timeManager.GetNow();
                _statusProvider.RaiseChanged();

                snapshot.Result = "Feasible transaction found - Converting...";
                snapshot.LastCheck = _timeManager.GetNow();
                _statusProvider.RaiseChanged();

                if (_client.TryConvert(sellAction, buyAction, _trading.ReferenceSymbol, cancellationToken, _timeManager.GetNow, out var transaction, out var error))
                {
                    snapshot.SellSymbol = transaction.Sell.SymbolName;
                    snapshot.SellPrice = transaction.Sell.QuoteQuantity;
                    snapshot.SellQuantity = transaction.Sell.Quantity;

                    snapshot.BuySymbol = transaction.Buy.SymbolName;
                    snapshot.BuyPrice = transaction.Buy.QuoteQuantity;
                    snapshot.BuyQuantity = transaction.Buy.Quantity;

                    snapshot.Result = $"Transaction done!";
                    snapshot.LastCheck = _timeManager.GetNow();
                    snapshot.LastSuccess = _timeManager.GetNow(); 
                    _statusProvider.RaiseChanged(StatusInfo.Important);

                    _statusProvider.Snapshot = snapshot = snapshot.ShallowClone();
                    using var data = new DataContext();
                    data.Entry(snapshot).State = EntityState.Added;
                    data.SaveChanges();

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