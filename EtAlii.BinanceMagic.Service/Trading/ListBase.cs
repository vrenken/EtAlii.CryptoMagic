namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using Microsoft.AspNetCore.Components;

    public abstract class ListBase<TSnapshot, TTrading> : ComponentBase, IDisposable
        where TSnapshot: class
        where TTrading : TradingBase, new()
    {
        [Inject] AlgorithmManager AlgorithmManager { get; init; }
        [Inject] NavigationManager NavigationManager { get; init; }
        [Inject] protected ApplicationContext ApplicationContext { get; init; }

        protected ObservableCollection<IAlgorithmRunner<TSnapshot, TTrading>> Tradings { get; } = new();
        
        protected abstract string GetViewNavigationUrl(Guid id);
        protected abstract string GetEditNavigationUrl();
                
        protected void OnEditSelected()
        {
            var navigationUrl = GetEditNavigationUrl();
            NavigationManager.NavigateTo(navigationUrl);
        }
        
        protected void OnTradingSelected(Guid tradingId)
        {
            var navigationUrl = GetViewNavigationUrl(tradingId);
            NavigationManager.NavigateTo(navigationUrl);
        }

        
        protected override void OnInitialized()
        {
            ReloadRunners();
            ((INotifyCollectionChanged)AlgorithmManager.Runners).CollectionChanged += OnRunnersChanged;
        }

        private void OnRunnersChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            InvokeAsync(() =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (var newItem in e.NewItems!.OfType<IAlgorithmRunner<TSnapshot, TTrading>>())
                        {
                            AddRunner(newItem);
                        }

                        break;
                    case NotifyCollectionChangedAction.Remove:
                        foreach (var oldItem in e.OldItems!.OfType<IAlgorithmRunner<TSnapshot, TTrading>>())
                        {
                            RemoveRunner(oldItem);
                        }

                        break;
                    case NotifyCollectionChangedAction.Reset:
                        Tradings.Clear();
                        ReloadRunners();
                        break;
                }
            });
        }
        
        
        private void ReloadRunners()
        {
            var runners = AlgorithmManager.Runners
                .OfType<IAlgorithmRunner<TSnapshot, TTrading>>()
                .ToArray();
            foreach (var runner in runners)
            {
                AddRunner(runner);
            }
        }
        private void RemoveRunner(IAlgorithmRunner<TSnapshot, TTrading> runner)
        {
            Tradings.Remove(runner);
        }

        private void AddRunner(IAlgorithmRunner<TSnapshot, TTrading> runner)
        {
            Tradings.Add(runner);
        }

        public void Dispose()
        {
            ((INotifyCollectionChanged)AlgorithmManager.Runners).CollectionChanged -= OnRunnersChanged;
        }
    }
}