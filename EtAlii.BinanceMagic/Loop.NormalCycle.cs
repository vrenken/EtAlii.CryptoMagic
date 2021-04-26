namespace EtAlii.BinanceMagic
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public partial class Loop
    {
        private bool TryHandleNormalCycle(CancellationToken cancellationToken, Target target, Situation situation, out bool targetSucceeded)
        {
            targetSucceeded = false;

            if (_algorithm.TransactionIsWorthIt(target, situation, out var sellAction, out var buyAction))
            {
                ConsoleOutput.WriteFormatted("Preparing sell action : {0, -40} (= {1})", $"{sellAction.Quantity} {sellAction.Coin}", $"{sellAction.Price} {_settings.ReferenceCoin}");
                ConsoleOutput.WriteFormatted("Preparing buy action  : {0, -40} (= {1})", $"{buyAction.Quantity} {buyAction.Coin}", $"{buyAction.Price} {_settings.ReferenceCoin}");

                ConsoleOutput.Write($"Feasible transaction found - Converting...");
                if (_client.TryConvert(sellAction, buyAction, _settings.ReferenceCoin, cancellationToken, out var transaction))
                {
                    ConsoleOutput.WritePositive($"Transaction done!");
                    ConsoleOutput.Write($"Next check at: {DateTime.Now + _settings.SampleInterval}");
                    _data.AddTransaction(transaction);
                    targetSucceeded = true;
                    Task.Delay(_settings.SampleInterval, cancellationToken).Wait(cancellationToken);
                }
            }

            return targetSucceeded;
        }
    }
}