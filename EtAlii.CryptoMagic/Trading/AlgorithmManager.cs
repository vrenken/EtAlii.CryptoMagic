namespace EtAlii.CryptoMagic
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;

    public partial class AlgorithmManager : IHostedService
    {
        private readonly ApplicationContext _applicationContext;
        public ReadOnlyObservableCollection<IAlgorithmRunner> Runners { get; }
        private readonly ObservableCollection<IAlgorithmRunner> _runners;

        public AlgorithmManager(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
            _runners = new ObservableCollection<IAlgorithmRunner>();
            Runners = new ReadOnlyObservableCollection<IAlgorithmRunner>(_runners);

            _runners.CollectionChanged += OnRunnersChanged;
        }

        private void OnRunnersChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var task = Task.Run(async () =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (IAlgorithmRunner runnerToStart in e.NewItems!)
                        {
                            await runnerToStart.Start();
                        }

                        break;
                    case NotifyCollectionChangedAction.Remove:
                        foreach (IAlgorithmRunner runnerToStop in e.OldItems!)
                        {
                            await runnerToStop.Stop();
                        }

                        break;
                }
            });
            task.Wait();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Run(Run, cancellationToken);
        }

        private void Run()
        {
            using var data = new DataContext();

            var allTradings = Array.Empty<TradingBase>();
            allTradings = allTradings
                .Concat(data.SimpleTradings.Cast<TradingBase>().ToArray())
                .Concat(data.CircularTradings.Cast<TradingBase>().ToArray())
                .Concat(data.OneOffTradings.Cast<TradingBase>().ToArray())
                .Concat(data.SurfingTradings.Cast<TradingBase>().ToArray())
                .Concat(data.EdgeTradings.Cast<TradingBase>().ToArray())
                .Concat(data.ExperimentalTradings.Cast<TradingBase>().ToArray())
                .ToArray();
            
            foreach (var trading in allTradings)
            {
                var runner = CreateRunner(trading);
                _runners.Add(runner);
            }
        }
        
        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var runner in Runners)
            {
                runner.Stop();
            }
            return Task.CompletedTask;
        }
    }
}