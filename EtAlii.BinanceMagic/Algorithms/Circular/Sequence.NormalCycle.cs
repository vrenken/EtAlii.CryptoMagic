namespace EtAlii.BinanceMagic.Circular
{
    using System.Threading;

    public partial class Sequence
    {
        private bool TryHandleNormalCycle(CancellationToken cancellationToken, Situation situation, out bool targetSucceeded)
        {
            targetSucceeded = false;

            if (_circularAlgorithm.TransactionIsWorthIt(situation, out var sellAction, out var buyAction))
            {
                _details.Result = $"Preparing to sell {sellAction.Quantity} {sellAction.Coin} and buy {buyAction.Quantity} {buyAction.Coin}";
                _details.LastCheck = _timeManager.GetNow();
                _statusProvider.RaiseChanged();

                _details.Result = "Feasible transaction found - Converting...";
                _details.LastCheck = _timeManager.GetNow();
                _statusProvider.RaiseChanged();

                if (_client.TryConvert(sellAction, buyAction, _settings.ReferenceCoin, cancellationToken, _timeManager.GetNow, out var transaction, out var error))
                {
                    _details.Result = $"Transaction done!";
                    _details.LastCheck = _timeManager.GetNow();
                    _details.LastSuccess = _timeManager.GetNow(); 
                    _statusProvider.RaiseChanged(StatusInfo.Important);

                    _data.Add(_details);
                    targetSucceeded = true;
                }
                else
                {
                    _details.Result = error;
                }
            }

            return targetSucceeded;
        }
    }
}