namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.IO;
    using System.Linq;

    public class CircularAlgorithmRunner : IAlgorithmRunner<CircularTrading>
    {
        public string Log => _output.Result;
        public CircularTrading Trading { get; }

        private readonly ApplicationContext _applicationContext;
        private readonly WebOutput _output;

        public CircularTradeSnapshot Status => _context.Snapshot;
        private IAlgorithmContext<CircularTradeSnapshot, CircularTrading> _context; 
        private Loop _loop;
        private IClient _client;
        private Sequence _sequence;

        public event Action Changed;
        
        public CircularAlgorithmRunner(CircularTrading trading, ApplicationContext applicationContext)
        {
            Trading = trading;
            _applicationContext = applicationContext;
            _output = new WebOutput();
        }

        public void Start()
        {
            ITimeManager time;

            var coins = new[] {Trading.FirstSymbol, Trading.SecondSymbol};

            using var data = new DataContext();
            var binanceApiKey = data.Settings.Single(s => s.Key == SettingKey.BinanceApiKey).Value;
            var binanceSecretKey = data.Settings.Single(s => s.Key == SettingKey.BinanceSecretKey).Value;

            var isBackTest = Trading.TradeMethod == TradeMethod.BackTest; 
            if (isBackTest)
            {
                var folder = GetType().Assembly.Location;
                folder = Path.GetDirectoryName(folder);
                
                var backTestClient = new BackTestClient(coins, _applicationContext.ReferenceSymbol, _output, Trading.Id, folder);
                _client = backTestClient;
                time = new BackTestTimeManager
                {
                    Client = backTestClient,
                };
            }
            else
            {
                var actionValidator = new ActionValidator();
                _client = new Client(actionValidator)
                {
                    PlaceTestOrders = Trading.TradeMethod == TradeMethod.BinanceTest
                };
                time = new RealtimeTimeManager();
            }

            var initialize = new Action(() => _client.Start(binanceApiKey, binanceSecretKey));
            
            var sampleInterval = isBackTest
                ? TimeSpan.FromSeconds(10)
                : (TimeSpan?)null;
            
            _context = new AlgorithmContext<CircularTradeSnapshot, CircularTrading>(sampleInterval)
            {
                Snapshot = new CircularTradeSnapshot
                {
                    Trading = Trading,
                }
            };
            _sequence = new Sequence(_client, time, Trading, _context, initialize);
            
            _sequence.Status.Changed += OnSequenceChanged;
            _loop = new Loop(_sequence);
            _loop.Start();
        }

        private void OnSequenceChanged(AlgorithmChange obj) => Changed?.Invoke();

        public void Stop()
        {
            _loop.Stop();
            _client.Stop();
            _sequence.Status.Changed -= OnSequenceChanged;
        }
    }
}