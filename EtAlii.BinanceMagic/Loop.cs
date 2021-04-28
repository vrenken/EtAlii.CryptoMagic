﻿namespace EtAlii.BinanceMagic
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

        private readonly TradeDetails _details;
        
        public Loop(LoopSettings settings, IProgram program, IClient client)
        {
            _settings = settings;
            _client = client;
            _data = new Data(_client, settings);
            _details = new TradeDetails();
            _algorithm = new Algorithm(settings, _data, program, _details);
            _targetBuilder = new TradeDetailsUpdater(_data, settings);
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
            _targetBuilder.UpdateTargetDetails(_details);
            
            var targetAchieved = false;
            var shouldDelay = false;

            while (!targetAchieved)
            {
                if (shouldDelay)
                {
                    var nextCheck = DateTime.Now + _settings.SampleInterval;
                    _details.NextCheck = nextCheck;
                    _details.DumpToConsole();
                    Task.Delay(_settings.SampleInterval, cancellationToken).Wait(cancellationToken);
                }
                _details.NextCheck = DateTime.MinValue;
                _details.Result = "Fetching current situation...";
                
                if (!_data.TryGetSituation(_details, cancellationToken, out var situation))
                {
                    shouldDelay = true;
                    continue;
                }
                
                _details.Result = "Fetching exchange info...";

                if (!_client.TryGetExchangeInfo(_details, cancellationToken, out var exchangeInfo))
                {
                    shouldDelay = true;
                    continue;
                }
                situation = situation with { ExchangeInfo = exchangeInfo };
                
                if (situation.IsInitialCycle)
                {
                    targetAchieved = HandleInitialCycle(cancellationToken, situation);
                    shouldDelay = !targetAchieved;
                    continue;
                }
                if(TryHandleNormalCycle(cancellationToken, situation, out targetAchieved))
                {
                    shouldDelay = !targetAchieved;
                    continue;
                }

                _details.Result = "No feasible transaction";
                shouldDelay = true;
            }
        }
    }
}