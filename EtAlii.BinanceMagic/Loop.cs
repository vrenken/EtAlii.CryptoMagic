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

        public StatusInfo Status => _status;
        private readonly StatusInfo _status;
        
        public Loop(LoopSettings settings, IProgram program, IClient client)
        {
            _settings = settings;
            _client = client;
            _data = new Data(_client, settings);
            _status = new StatusInfo();
            _algorithm = new Algorithm(settings, _data, program, _status);
            _targetBuilder = new TargetBuilder(_data, settings, _status);
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
            Status.FromCoin = target.Source;
            Status.ToCoin = target.Destination;
            Status.ReferenceCoin = _settings.ReferenceCoin;
            Status.Result = "Found next target";
            
            var targetSucceeded = false;
            var shouldDelay = false;

            while (!targetSucceeded)
            {
                if (shouldDelay)
                {
                    var nextCheck = DateTime.Now + _settings.SampleInterval;
                    Status.NextCheck = nextCheck;
                    Status.DumpToConsole();
                    Task.Delay(_settings.SampleInterval, cancellationToken).Wait(cancellationToken);
                }
                Status.NextCheck = DateTime.MinValue;
                Status.Result = "Fetching current situation...";
                
                if (!_data.TryGetSituation(target, _status, cancellationToken, out var situation))
                {
                    shouldDelay = true;
                    continue;
                }
                
                Status.Result = "Fetching exchange info...";

                if (!_client.TryGetExchangeInfo(_status, cancellationToken, out var exchangeInfo))
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

                Status.Result = "No feasible transaction";
                shouldDelay = true;
            }
        }
    }
}