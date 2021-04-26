namespace EtAlii.BinanceMagic
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public partial class Loop
    {
        private readonly LoopSettings _settings;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private Task _task;
        private readonly IAlgorithm _algorithm;
        private readonly IData _data;
        private readonly ITargetBuilder _targetBuilder;
        private readonly IClient _client;
        private static readonly object LockObject = new();

        public StatusInfo Status { get; } = new ();
        
        public Loop(LoopSettings settings, IProgram program, IClient client)
        {
            _settings = settings;
            _client = client;
            _data = new Data(_client, settings);
            _algorithm = new Algorithm(settings, _data, program);
            _targetBuilder = new TargetBuilder(_data, settings);
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

            _data.Load();

            while (!cancellationToken.IsCancellationRequested)
            {
                lock (LockObject)
                {
                    RunOnce(cancellationToken);
                }
            }

            _client.Stop();
        }

        private void RunOnce(CancellationToken cancellationToken)
        {
            var target = _targetBuilder.BuildTarget();
            ConsoleOutput.Write($"Found next target: {target.Source} -> {target.Destination} using minimal increase: {_settings.MinimalIncrease:P}");
            Status.FromCoin = target.Source;
            Status.ToCoin = target.Destination;
            
            var targetSucceeded = false;
            var shouldDelay = false;

            while (!targetSucceeded)
            {
                if (shouldDelay)
                {
                    var nextCheck = DateTime.Now + _settings.SampleInterval;
                    ConsoleOutput.WriteNegative($"Waiting until: {nextCheck}");
                    Status.NextCheck = nextCheck;
                }
                if (!_data.TryGetSituation(target, cancellationToken, out var situation))
                {
                    shouldDelay = true;
                    continue;
                }
                
                if (!_client.TryGetExchangeInfo(cancellationToken, out var exchangeInfo))
                {
                    shouldDelay = true;
                    continue;
                }
                situation = situation with { ExchangeInfo = exchangeInfo };
                
                if (situation.IsInitialCycle)
                {
                    targetSucceeded = HandleInitialCycle(cancellationToken, target, situation);
                    shouldDelay = !targetSucceeded;
                    continue;
                }
                if(TryHandleNormalCycle(cancellationToken, target, situation, out targetSucceeded))
                {
                    shouldDelay = !targetSucceeded;
                    continue;
                }

                ConsoleOutput.WriteNegative($"No feasible transaction");
                Status.Result = $"No feasible transaction";
                shouldDelay = true;
            }
        }
    }
}