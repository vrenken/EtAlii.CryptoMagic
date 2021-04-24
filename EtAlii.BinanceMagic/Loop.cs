namespace EtAlii.BinanceMagic
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class Loop
    {
        private readonly Settings _settings;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private Task _task;
        private readonly Algorithm _algorithm;
        private readonly Data _data;
        private readonly Client _client;

        public Loop(Settings settings, Program program)
        {
            _settings = settings;
            _client = new Client(settings, program);
            _algorithm = new Algorithm(_client, settings);
            _data = new Data(_client, settings);
        }
        
        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _task.Wait();
        }

        public void Start()
        {
            _task = Task.Run(Run);
        }
        private void Run()
        {
            var cancellationToken = _cancellationTokenSource.Token;

            _client.Start();
            _data.Load();

            while (!cancellationToken.IsCancellationRequested)
            {
                var target = _data.BuildTarget();
                ConsoleOutput.Write($"Found next target: {target.Source} -> {target.Destination} using minimal increase: {_settings.MinimalIncrease:P}");

                var targetSucceeded = false;

                while (!targetSucceeded)
                {
                    var situation = _data.GetSituation(target, cancellationToken);

                    if (situation.IsInitialCycle)
                    {
                        ConsoleOutput.Write($"Initial cycle - Converting...");
                        _algorithm.ToInitialConversionActions(target, cancellationToken, out var initialSellAction, out var initialBuyAction);
                        ConsoleOutput.WriteFormatted("Preparing sell action : {0, -40} (= {1})", $"{initialSellAction.Quantity} {initialSellAction.Coin}", $"{initialSellAction.Price} {_settings.ReferenceCoin}");
                        ConsoleOutput.WriteFormatted("Preparing buy action  : {0, -40} (= {1})", $"{initialBuyAction.Quantity} {initialBuyAction.Coin}", $"{initialBuyAction.Price} {_settings.ReferenceCoin}");

                        if (_client.TryConvert(initialSellAction, initialBuyAction, cancellationToken, out var transaction))
                        {
                            ConsoleOutput.WritePositive($"Transaction done!");
                            _data.AddTransaction(transaction);
                            targetSucceeded = true;
                        }
                    }
                    else if (_algorithm.TransactionIsWorthIt(target, situation, out var sellAction, out var buyAction))
                    {
                        ConsoleOutput.WriteFormatted("Preparing sell action : {0, -40} (= {1})", $"{sellAction.Quantity} {sellAction.Coin}", $"{sellAction.Price} {_settings.ReferenceCoin}");
                        ConsoleOutput.WriteFormatted("Preparing buy action  : {0, -40} (= {1})", $"{buyAction.Quantity} {buyAction.Coin}", $"{buyAction.Price} {_settings.ReferenceCoin}");

                        ConsoleOutput.Write($"Feasible transaction found - Converting...");
                        if (_client.TryConvert(sellAction, buyAction, cancellationToken, out var transaction))
                        {
                            ConsoleOutput.WritePositive($"Transaction done!");
                            ConsoleOutput.Write("Next check at: {DateTime.Now + _settings.SampleInterval}");
                            _data.AddTransaction(transaction);
                            targetSucceeded = true;
                            Task.Delay(_settings.SampleInterval, cancellationToken).Wait(cancellationToken);
                        }
                    }
                    else
                    {
                        ConsoleOutput.WriteNegative($"No feasible transaction - Waiting until: {DateTime.Now + _settings.SampleInterval}");
                        Task.Delay(_settings.SampleInterval, cancellationToken).Wait(cancellationToken);
                    }
                }
            }

            _client.Stop();
        }
    }
}