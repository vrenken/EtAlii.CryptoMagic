namespace EtAlii.BinanceMagic
{
    using System;
    using System.Threading;

    public partial class CircularSequence : ISequence
    {
        private readonly CircularAlgorithmSettings _settings;
        private readonly ICircularAlgorithm _circularAlgorithm;
        private readonly ITradeDetailsBuilder _detailsBuilder;
        private readonly TradeDetails _details;
        private readonly ICircularData _data;
        private readonly IClient _client;
        private readonly ITimeManager _timeManager;

        public IStatusProvider Status => _statusProvider;
        private readonly StatusProvider _statusProvider;

        public CircularSequence(
            CircularAlgorithmSettings settings, IProgram program, 
            IClient client, IOutput output,
            ITimeManager timeManager)
        {
            _settings = settings;
            _client = client;
            _timeManager = timeManager;
            _data = new CircularData(_client, settings, output);
            _details = new TradeDetails();
            _statusProvider = new StatusProvider(output, _details);
            _detailsBuilder = new TradeDetailsUpdater(_data, settings);
            _circularAlgorithm = new CircularAlgorithm(settings, _data, program, client, _details, _statusProvider);
        }

        public void Initialize()
        {
            _data.Load();
        }
        
        public void Run(CancellationToken cancellationToken)
        {
            _detailsBuilder.UpdateTargetDetails(_details);
            _details.LastCheck = _timeManager.GetNow();
            _statusProvider.RaiseChanged();
            
            var targetAchieved = false;
            var shouldDelay = false;

            while (!targetAchieved)
            {
                if (shouldDelay)
                {
                    _details.NextCheck = _timeManager.GetNow() + _settings.SampleInterval;

                    var isTransition = _details.NextCheck.Hour != _details.LastCheck.Hour;
                    _statusProvider.RaiseChanged(isTransition ? StatusInfo.Important : StatusInfo.Normal);

                    _timeManager.Wait(_settings.SampleInterval, cancellationToken);
                }
                _details.NextCheck = DateTime.MinValue;
                _details.Result = "Fetching current situation...";
                _details.LastCheck = _timeManager.GetNow();
                _statusProvider.RaiseChanged();

                if (!_data.TryGetSituation(_details, cancellationToken, out var situation))
                {
                    shouldDelay = true;
                    continue;
                }
                
                _details.Result = "Fetching exchange info...";
                _details.LastCheck = _timeManager.GetNow();
                _statusProvider.RaiseChanged();

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
                _details.LastCheck = _timeManager.GetNow();
                _statusProvider.RaiseChanged();
                shouldDelay = true;
            }
        }
    }
}