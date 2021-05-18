namespace EtAlii.BinanceMagic.Service
{
    using System.Linq;

    public class CircularAlgorithmRunner : IAlgorithmRunner
    {
        public string Log => _output.Result;
        public TradingBase Trading => _trading;
        private readonly CircularTrading _trading;
        private readonly WebOutput _output;
        private Loop _loop;
        private IClient _client;

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
                var backTestClient = new BackTestClient(coins, referenceSymbol, _output, "bin\\Debug\\net5.0");
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
            
            var algorithmSettings = new Circular.AlgorithmSettings
            {
                AllowedCoins = coins,
                ReferenceCoin = referenceSymbol,
                TargetIncrease = _trading.TargetIncrease,
                QuantityFactor = _trading.QuantityFactor,
                InitialTarget = _trading.InitialTarget,
                SampleInterval = _trading.SampleInterval,
            };
            
            var tradeDetailsPersistence = new Persistence<Circular.TradeDetails>("TradeDetailsStorage", _trading.Name);
            var sequence = new Circular.Sequence(algorithmSettings, _client, _output, time, tradeDetailsPersistence);
            _loop = new Loop(sequence);
            _loop.Start();
        }

        public void Stop()
        {
            _loop.Stop();
            _client.Stop();
        }
    }
}