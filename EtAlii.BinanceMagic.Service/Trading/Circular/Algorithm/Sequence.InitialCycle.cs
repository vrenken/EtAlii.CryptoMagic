namespace EtAlii.BinanceMagic.Service
{
    using System.Threading;
    using Microsoft.EntityFrameworkCore;

    public partial class Sequence
    {
        private bool HandleInitialCycle(CancellationToken cancellationToken, Situation situation)
        {
            var snapshot = _statusProvider.Snapshot;
            
            var targetSucceeded = false;
            
            snapshot.Result = "Initial cycle";
            snapshot.LastCheck = _timeManager.GetNow();
            _statusProvider.RaiseChanged();

            _circularAlgorithm.ToInitialConversionActions(situation, out var initialSellAction, out var initialBuyAction);
            snapshot.Result = $"Preparing to sell {initialSellAction.Quantity} {initialSellAction.Symbol} and buy {initialBuyAction.Quantity} {initialBuyAction.Symbol}";
            snapshot.LastCheck = _timeManager.GetNow();
            _statusProvider.RaiseChanged();

            if (_client.TryConvert(initialSellAction, initialBuyAction, _trading.ReferenceSymbol, cancellationToken, _timeManager.GetNow, out var transaction, out var error))
            {
                snapshot.SellSymbol = transaction.Sell.SymbolName;
                snapshot.SellPrice = transaction.Sell.Price;
                snapshot.SellQuantity = transaction.Sell.Quantity;

                snapshot.BuySymbol = transaction.Buy.SymbolName;
                snapshot.BuyPrice = transaction.Buy.Price;
                snapshot.BuyQuantity = transaction.Buy.Quantity;

                snapshot.Result = "Transaction done";
                snapshot.LastCheck = _timeManager.GetNow();
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
            return targetSucceeded;
        }
    }
}