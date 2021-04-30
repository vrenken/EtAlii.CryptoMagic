namespace EtAlii.BinanceMagic
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public partial class CircularSequence : ISequence
    {
        private readonly CircularAlgorithmSettings _settings;
        private readonly ICircularAlgorithm _circularAlgorithm;
        private readonly ITradeDetailsBuilder _detailsBuilder;
        private readonly TradeDetails _details;
        private readonly ICircularData _data;
        private readonly IClient _client;

        public IStatusProvider Status => _statusProvider;
        private readonly StatusProvider _statusProvider;

        public CircularSequence(CircularAlgorithmSettings settings, IProgram program, IClient client, IOutput output)
        {
            _settings = settings;
            _client = client;
            _data = new CircularData(_client, settings, output);
            _details = new TradeDetails();
            _statusProvider = new StatusProvider(output, _details);
            _details.Updated += _statusProvider.Write;
            _detailsBuilder = new TradeDetailsUpdater(_data, settings);
            
            _circularAlgorithm = new CircularAlgorithm(settings, _data, program, _details, _statusProvider);
        }

        public void Initialize()
        {
            _data.Load();
        }
        
        public void Run(CancellationToken cancellationToken)
        {
            _detailsBuilder.UpdateTargetDetails(_details);
            
            var targetAchieved = false;
            var shouldDelay = false;

            while (!targetAchieved)
            {
                if (shouldDelay)
                {
                    var nextCheck = DateTime.Now + _settings.SampleInterval;
                    _details.NextCheck = nextCheck;
                    _statusProvider.RaiseChanged();

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