namespace EtAlii.BinanceMagic
{
    using System.Threading;

    public partial class CircularSequence
    {
        private bool TryHandleNormalCycle(CancellationToken cancellationToken, Situation situation, out bool targetSucceeded)
        {
            targetSucceeded = false;

            if (_circularAlgorithm.TransactionIsWorthIt(situation, out var sellAction, out var buyAction))
            {
                _details.Result = $"Preparing to sell {sellAction.Quantity} {sellAction.Coin} and buy {buyAction.Quantity} {buyAction.Coin}";
                _details.LastCheck = _timeManager.GetNow();
                _statusProvider.RaiseChanged();

                // ConsoleOutput.WriteFormatted("Preparing sell action : {0, -40} (= {1})", $"{sellAction.Quantity} {sellAction.Coin}", $"{sellAction.Price} {_settings.ReferenceCoin}");
                // ConsoleOutput.WriteFormatted("Preparing buy action  : {0, -40} (= {1})", $"{buyAction.Quantity} {buyAction.Coin}", $"{buyAction.Price} {_settings.ReferenceCoin}");

                _details.Result = "Feasible transaction found - Converting...";
                _details.LastCheck = _timeManager.GetNow();
                _statusProvider.RaiseChanged();

                if (_client.TryConvert(sellAction, buyAction, _settings.ReferenceCoin, _details, cancellationToken, _timeManager.GetNow, out var transaction))
                {
                    _details.Result = $"Transaction done!";
                    _details.LastCheck = _timeManager.GetNow();
                    _details.LastSuccess = _timeManager.GetNow(); 
                    _statusProvider.RaiseChanged();

                    _data.AddTransaction(transaction);
                    targetSucceeded = true;
                }
            }

            return targetSucceeded;
        }
    }
}