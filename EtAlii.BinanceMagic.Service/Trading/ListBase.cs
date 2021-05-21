namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using Microsoft.AspNetCore.Components;

    public abstract class ListBase<TTrading> : ComponentBase, IDisposable
        where TTrading : TradingBase, new()
    {
        [Inject] AlgorithmManager AlgorithmManager { get; init; }
        [Inject] NavigationManager NavigationManager { get; init; }
        [Inject] protected ApplicationContext ApplicationContext { get; init; }

        protected ObservableCollection<IAlgorithmRunner> Tradings { get; } = new();
        
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
                        foreach (IAlgorithmRunner newItem in e.NewItems!)
                        {
                            AddRunner(newItem);
                        }

                        break;
                    case NotifyCollectionChangedAction.Remove:
                        foreach (IAlgorithmRunner oldItem in e.OldItems!)
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
            var runners = AlgorithmManager.Runners.ToArray();
            foreach (var runner in runners)
            {
                AddRunner(runner);
            }
        }
        private void RemoveRunner(IAlgorithmRunner runner)
        {
            if (runner.Trading is TTrading)
            {
                Tradings.Remove(runner);
            }
        }

        private void AddRunner(IAlgorithmRunner runner)
        {
            if (runner.Trading is TTrading)
            {
                Tradings.Add(runner);
            }            
        }
        public void Dispose()
        {
            ((INotifyCollectionChanged)AlgorithmManager.Runners).CollectionChanged -= OnRunnersChanged;
        }
    }
}