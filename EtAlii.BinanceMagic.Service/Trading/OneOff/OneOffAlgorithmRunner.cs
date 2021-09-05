namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public class OneOffAlgorithmRunner : IAlgorithmRunner<OneOffTransaction, OneOffTrading>
    {
        private Client _client;
        private readonly OneOffTrading _trading;
        public event Action<IAlgorithmRunner<OneOffTransaction, OneOffTrading>> Changed;
        public string Log { get; } = string.Empty;

        private Timer _timer;
        public IAlgorithmContext<OneOffTransaction, OneOffTrading> Context { get; }

        public OneOffAlgorithmRunner(OneOffTrading trading)
        {
            _trading = trading;
            Context = new AlgorithmContext<OneOffTransaction, OneOffTrading>(trading);
        }
        public async Task Start()
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
            
            _timer = new Timer(RunStep, null, TimeSpan.FromSeconds(30), _trading.SampleInterval);
                
            Changed?.Invoke(this);
        }

        private void RunStep(object state)
        {
            Changed?.Invoke(this);
        }

        public async Task Stop()
        {
            await _timer.DisposeAsync();
            Changed?.Invoke(this);
        }

        public void Cancel()
        {
            var task = Stop();
            task.Wait();
            
            using var data = new DataContext();

            Context.Trading.IsCancelled = true;
            data.OneOffTradings.Attach(Context.Trading).State = EntityState.Modified;
            data.SaveChanges();
            
            Changed?.Invoke(this);
        }
    }
}