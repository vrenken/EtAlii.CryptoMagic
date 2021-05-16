namespace EtAlii.BinanceMagic.Service.Trading
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;

    public partial class AlgorithmManager : IHostedService
    {
        private CancellationTokenSource _cancellationTokenSource;
        private Task _task;
        private CancellationToken _cancellationToken;

        public ObservableCollection<IAlgorithmRunner> Runners { get; } = new();
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _task = Task.Run(Run, cancellationToken);
            return Task.CompletedTask;
        }

        private void Run()
        {
            var data = new DataContext();

            var allTradings = Array.Empty<TradingBase>();
            allTradings = allTradings
                .Concat(data.SimpleTradings.Cast<TradingBase>().ToArray())
                .Concat(data.CircularTradings.Cast<TradingBase>().ToArray())
                .Concat(data.SurfingTradings.Cast<TradingBase>().ToArray())
                .Concat(data.ExperimentalTradings.Cast<TradingBase>().ToArray())
                .ToArray();
            
            foreach (var trading in allTradings)
            {
                var runner = CreateRunner(trading);
                Runners.Add(runner);
            }
        }
        
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            _task.Wait(CancellationToken.None);
            return Task.CompletedTask;
        }
    }
}