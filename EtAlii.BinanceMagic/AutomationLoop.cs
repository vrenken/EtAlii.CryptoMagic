namespace EtAlii.BinanceMagic
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class AutomationLoop
    {
        private readonly Settings _settings;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private Task _task;
        private readonly MagicAlgorithm _algorithm;
        private readonly DataProvider _data;
        private readonly Client _client;

        public AutomationLoop(Settings settings)
        {
            _settings = settings;
            _client = new Client(settings);
            _algorithm = new MagicAlgorithm();
            _data = new DataProvider(_client, settings);
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
                        if (_client.TryConvert(target, situation, out var transaction))
                        {
                            ConsoleOutput.WritePositive($"Transaction done!");
                            _data.AddTransaction(transaction);
                            targetSucceeded = true;
                        }
                    }
                    if (_algorithm.TransactionIsWorthIt(target, situation))
                    {
                        ConsoleOutput.Write($"Feasible transaction found - Converting...");
                        if (_client.TryConvert(target, situation, out var transaction))
                        {
                            ConsoleOutput.WritePositive($"Transaction done!");
                            _data.AddTransaction(transaction);
                            targetSucceeded = true;
                        }
                    }
                    else
                    {
                        var interval = _settings.SampleInterval;
                        ConsoleOutput.WriteNegative($"No feasible transaction - Waiting until: {DateTime.Now + interval}");
                        Task.Delay(interval, cancellationToken).Wait(cancellationToken);
                    }
                }
            }

            _client.Stop();
        }
    }
}