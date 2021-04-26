namespace EtAlii.BinanceMagic
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public partial class Loop
    {
        private bool HandleInitialCycle(CancellationToken cancellationToken, Target target, Situation situation)
        {
            var targetSucceeded = false;
            
            ConsoleOutput.Write($"Initial cycle - Converting...");
            Status.Result = "Initial cycle";

            _algorithm.ToInitialConversionActions(target, situation, out var initialSellAction, out var initialBuyAction);
            ConsoleOutput.WriteFormatted("Preparing sell action : {0, -40} (= {1})", $"{initialSellAction.Quantity} {initialSellAction.Coin}", $"{initialSellAction.Price} {_settings.ReferenceCoin}");
            ConsoleOutput.WriteFormatted("Preparing buy action  : {0, -40} (= {1})", $"{initialBuyAction.Quantity} {initialBuyAction.Coin}", $"{initialBuyAction.Price} {_settings.ReferenceCoin}");

            if (_client.TryConvert(initialSellAction, initialBuyAction, _settings.ReferenceCoin, cancellationToken, out var transaction))
            {
                transaction = transaction with {TotalProfit = 0m};

                ConsoleOutput.WritePositive($"Transaction done!");
                _data.AddTransaction(transaction);
                targetSucceeded = true;
            }
            else
            {
                ConsoleOutput.WriteNegative($"Waiting until: {DateTime.Now + _settings.SampleInterval}");
                Task.Delay(_settings.SampleInterval, cancellationToken).Wait(cancellationToken);
            }

            return targetSucceeded;
        }
    }
}