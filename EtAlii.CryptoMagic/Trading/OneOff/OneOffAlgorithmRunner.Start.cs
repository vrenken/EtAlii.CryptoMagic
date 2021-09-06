namespace EtAlii.CryptoMagic
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public partial class OneOffAlgorithmRunner 
    {
        public async Task Start()
        {
            if (!_trading.IsSuccess && !_trading.IsCancelled)
            {
                await StartClientWhenNeeded();            

                _timer = new Timer(RunStepInternal, null, TimeSpan.FromSeconds(30), _trading.SampleInterval);
            }
                
            Changed?.Invoke(this);
        }

        private void RunStepInternal(object state)
        {
            try
            {
                var task = Task.Run(async () => await RunStep());
                task.Wait();
            }
            catch (Exception)
            {
                // Let's do nothing right now.
            }
        }

        private async Task StartClientWhenNeeded()
        {
            if (_client == null)
            {
                var actionValidator = new ActionValidator();
                _client = new Client(actionValidator)
                {
                    PlaceTestOrders = _trading.TradeMethod == TradeMethod.BinanceTest
                };

                await using var data = new DataContext();
                var binanceApiKey = data.Settings.Single(s => s.Key == SettingKey.BinanceApiKey).Value;
                var binanceSecretKey = data.Settings.Single(s => s.Key == SettingKey.BinanceSecretKey).Value;
                await _client.Start(binanceApiKey, binanceSecretKey);
                
            }

        }
    }
}