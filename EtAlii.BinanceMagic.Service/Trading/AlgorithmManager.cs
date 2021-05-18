namespace EtAlii.BinanceMagic.Service
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
        public ReadOnlyObservableCollection<IAlgorithmRunner> Runners { get; }
        private readonly ObservableCollection<IAlgorithmRunner> _runners;

        public AlgorithmManager()
        {
            _runners = new ObservableCollection<IAlgorithmRunner>();
            Runners = new ReadOnlyObservableCollection<IAlgorithmRunner>(_runners);

            _runners.CollectionChanged += OnRunnersChanged;
        }

        private void OnRunnersChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (IAlgorithmRunner newItem in e.NewItems!)
                    {
                        newItem.Start();
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (IAlgorithmRunner oldItem in e.OldItems!)
                    {
                        oldItem.Stop();
                    }
                    break;
            }
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
                .Concat(data.SurfingTradings.Cast<TradingBase>().ToArray())
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