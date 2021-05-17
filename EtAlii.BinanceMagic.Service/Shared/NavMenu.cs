namespace EtAlii.BinanceMagic.Service.Shared
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Threading.Tasks;
    using EtAlii.BinanceMagic.Service.Trading;
    using EtAlii.BinanceMagic.Service.Trading.Circular;
    using EtAlii.BinanceMagic.Service.Trading.Experimental;
    using EtAlii.BinanceMagic.Service.Trading.OneOff;
    using EtAlii.BinanceMagic.Service.Trading.Simple;
    using EtAlii.BinanceMagic.Service.Trading.Surfing;
    using Microsoft.AspNetCore.Components;
    using Microsoft.EntityFrameworkCore;

    public partial class NavMenu : IDisposable
    {
        private ObservableCollection<IAlgorithmRunner> OneOffTradings { get; } = new();
        private ObservableCollection<IAlgorithmRunner> CircularTradings { get; } = new();
        private ObservableCollection<IAlgorithmRunner> SimpleTradings { get; } = new();
        private ObservableCollection<IAlgorithmRunner> SurfingTradings { get; } = new();
        private ObservableCollection<IAlgorithmRunner> ExperimentalTradings { get; } = new();

        protected override void OnInitialized()
        {
            ReloadRunners();

            _algorithmManager.Runners.CollectionChanged += OnRunnersChanged;
        }

        private void OnRunnersChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            InvokeAsync(() =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (IAlgorithmRunner newItem in e.NewItems!)
                        {
                            AddRunner(newItem);
                        }

                        break;
                    case NotifyCollectionChangedAction.Remove:
                        foreach (IAlgorithmRunner newItem in e.OldItems!)
                        {
                            RemoveRunner(newItem);
                        }

                        break;
                    case NotifyCollectionChangedAction.Reset:
                        OneOffTradings.Clear();
                        CircularTradings.Clear();
                        SimpleTradings.Clear();
                        SurfingTradings.Clear();
                        ExperimentalTradings.Clear();
                        ReloadRunners();
                        break;
                }
            });
        }

        private void ReloadRunners()
        {
            var runners = _algorithmManager.Runners
                .ToArray();
            foreach (var runner in runners)
            {
                AddRunner(runner);
            }
        }
        private void RemoveRunner(IAlgorithmRunner runner)
        {
            switch (runner.Trading)
            {
                case OneOffTrading:
                    OneOffTradings.Remove(runner);
                    break;
                case CircularTrading:
                    CircularTradings.Remove(runner);
                    break;
                case SimpleTrading:
                    SimpleTradings.Remove(runner);
                    break;
                case SurfingTrading:
                    SurfingTradings.Remove(runner);
                    break;
                case ExperimentalTrading:
                    ExperimentalTradings.Remove(runner);
                    break;
            }
        }

        private void AddRunner(IAlgorithmRunner runner)
        {
            switch (runner.Trading)
            {
                case OneOffTrading:
                    OneOffTradings.Add(runner);
                    break;
                case CircularTrading:
                    CircularTradings.Add(runner);
                    break;
                case SimpleTrading:
                    SimpleTradings.Add(runner);
                    break;
                case SurfingTrading:
                    SurfingTradings.Add(runner);
                    break;
                case ExperimentalTrading:
                    ExperimentalTradings.Add(runner);
                    break;
            }            
        }
        public void Dispose()
        {
            _algorithmManager.Runners.CollectionChanged -= OnRunnersChanged;
        }
    }
}