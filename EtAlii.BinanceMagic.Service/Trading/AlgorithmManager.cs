namespace EtAlii.BinanceMagic.Service.Trading
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;

    public class AlgorithmManager : IHostedService
    {
        private CancellationTokenSource _cancellationTokenSource;
        private Task _task;
        private CancellationToken _cancellationToken;

        public event Action Tick;

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
            while (!_cancellationToken.IsCancellationRequested)
            {
                Tick?.Invoke();
                Task.Delay(TimeSpan.FromSeconds(10), _cancellationToken).Wait(_cancellationToken);
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