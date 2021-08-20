namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class CircularAlgorithmRunner : IAlgorithmRunner<CircularTransaction, CircularTrading>
    {
        public string Log => _output.Result;
        private readonly CircularTrading _trading;

        private readonly ApplicationContext _applicationContext;
        private readonly WebOutput _output;

        public IAlgorithmContext<CircularTransaction, CircularTrading> Context { get; private set; } 
        private Loop _loop;
        private IClient _client;
        private Sequence _sequence;

        public event Action Changed;
        
        public CircularAlgorithmRunner(CircularTrading trading, ApplicationContext applicationContext)
        {
            _trading = trading;
            _applicationContext = applicationContext;
            _output = new WebOutput();
        }

        public void Start()
        {
            ITimeManager time;

            var coins = new[] {_trading.FirstSymbol, _trading.SecondSymbol};

            using var data = new DataContext();
            var binanceApiKey = data.Settings.Single(s => s.Key == SettingKey.BinanceApiKey).Value;
            var binanceSecretKey = data.Settings.Single(s => s.Key == SettingKey.BinanceSecretKey).Value;

            var isBackTest = _trading.TradeMethod == TradeMethod.BackTest; 
            if (isBackTest)
            {
                var folder = GetType().Assembly.Location;
                folder = Path.GetDirectoryName(folder);
                
                var backTestClient = new BackTestClient(coins, _applicationContext.ReferenceSymbol, _output, _trading.Id, folder);
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
                    PlaceTestOrders = _trading.TradeMethod == TradeMethod.BinanceTest
                };
                time = new RealtimeTimeManager();
            }

            var initialize = new Func<Task>(() => _client.Start(binanceApiKey, binanceSecretKey));
            
            var sampleInterval = isBackTest
                ? TimeSpan.FromSeconds(10)
                : (TimeSpan?)null;

            Context = new AlgorithmContext<CircularTransaction, CircularTrading>(_trading, sampleInterval);
            _sequence = new Sequence(_client, time, Context, initialize);
            
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