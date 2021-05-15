namespace EtAlii.BinanceMagic.Circular
{
    using System.Threading;

    public partial class Sequence
    {
        private bool HandleInitialCycle(CancellationToken cancellationToken, Situation situation)
        {
            var targetSucceeded = false;
            
            _details.Result = "Initial cycle";
            _details.LastCheck = _timeManager.GetNow();
            _statusProvider.RaiseChanged();

            _circularAlgorithm.ToInitialConversionActions(situation, out var initialSellAction, out var initialBuyAction);
            _details.Result = $"Preparing to sell {initialSellAction.Quantity} {initialSellAction.Coin} and buy {initialBuyAction.Quantity} {initialBuyAction.Coin}";
            _details.LastCheck = _timeManager.GetNow();
            _statusProvider.RaiseChanged();

            if (_client.TryConvert(initialSellAction, initialBuyAction, _settings.ReferenceCoin, cancellationToken, _timeManager.GetNow, out var transaction, out var error))
            {
                _details.Result = "Transaction done";
                _details.LastCheck = _timeManager.GetNow();
                _statusProvider.RaiseChanged(StatusInfo.Important);
                _data.Add(_details);
                targetSucceeded = true;
            }
            else
            {
                _details.Result = error;
            }
            return targetSucceeded;
        }
    }
}