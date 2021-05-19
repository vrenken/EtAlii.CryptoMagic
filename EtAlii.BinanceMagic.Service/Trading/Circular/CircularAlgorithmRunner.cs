namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Linq;

    public class CircularAlgorithmRunner : IAlgorithmRunner
    {
        public string Log => _output.Result;
        public TradingBase Trading => _trading;
        private readonly CircularTrading _trading;
        private readonly WebOutput _output;

        public CircularTradeSnapshot Status => _context.Snapshot;
        private IAlgorithmContext<CircularTradeSnapshot> _context; 
        private Loop _loop;
        private IClient _client;
        private Sequence _sequence;

        public event Action Changed;
        
        public CircularAlgorithmRunner(CircularTrading trading)
        {
            _trading = trading;
            _output = new WebOutput();
        }

        public void Start()
        {
            ITimeManager time;

            var coins = new[] {_trading.FirstSymbol, _trading.SecondSymbol};

            using var data = new DataContext();
            var binanceApiKey = data.Settings.Single(s => s.Key == SettingKey.BinanceApiKey).Value;
            var binanceSecretKey = data.Settings.Single(s => s.Key == SettingKey.BinanceSecretKey).Value;
            var referenceSymbol = data.Settings.Single(s => s.Key == SettingKey.ReferenceSymbol).Value;
            
            if (_trading.Connectivity == Connectivity.BackTest)
            {
                var backTestClient = new BackTestClient(coins, referenceSymbol, _output);
                _client = backTestClient;
                time = new BackTestTimeManager
                {
                    Client = backTestClient,
                    Output = _output,
                    TerminateProcessWhenCompleted = false,
                };
            }
            else
            {
                var actionValidator = new ActionValidator();
                _client = new Client(actionValidator)
                {
                    PlaceTestOrders = _trading.Connectivity == Connectivity.Test
                };
                time = new RealtimeTimeManager();
            }

            _client.Start(binanceApiKey, binanceSecretKey);

            var sampleInterval = _trading.Connectivity == Connectivity.BackTest
                ? TimeSpan.FromSeconds(10)
                : (TimeSpan?)null;
            
            _context = new AlgorithmContext<CircularTradeSnapshot>(sampleInterval)
            {
                Snapshot = new CircularTradeSnapshot
                {
                    Trading = _trading,
                }
            };
            _sequence = new Sequence(_client, time, _trading, _context);
            _sequence.Status.Changed += OnSequenceChanged;
            _loop = new Loop(_sequence);
            _loop.Start();
        }

        private void OnSequenceChanged(StatusInfo obj) => Changed?.Invoke();

        public void Stop()
        {
            _loop.Stop();
            _client.Stop();
            _sequence.Status.Changed -= OnSequenceChanged;
        }
    }
}