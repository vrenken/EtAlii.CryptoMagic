namespace EtAlii.CryptoMagic
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using Microsoft.AspNetCore.Components;

    public abstract class ListBase<TTransaction, TTrading> : ComponentBase, IDisposable
        where TTransaction: TransactionBase<TTrading>
        where TTrading : TradingBase, new()
    {
        [Inject] AlgorithmManager AlgorithmManager { get; init; }
        [Inject] protected NavigationManager NavigationManager { get; init; }
        [Inject] protected ApplicationContext ApplicationContext { get; init; }

        private readonly ObservableCollection<IAlgorithmRunner<TTransaction, TTrading>> _tradings = new();

        protected readonly ObservableCollection<IAlgorithmRunner<TTransaction, TTrading>> Tradings = new();

        protected ListBase()
        {
            Tradings.SubscribeFiltered(_tradings, Filter);
        }

        protected virtual bool Filter(IAlgorithmRunner<TTransaction, TTrading> runner) => true;
        
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
                        foreach (var newItem in e.NewItems!.OfType<IAlgorithmRunner<TTransaction, TTrading>>())
                        {
                            AddRunner(newItem);
                        }

                        break;
                    case NotifyCollectionChangedAction.Remove:
                        foreach (var oldItem in e.OldItems!.OfType<IAlgorithmRunner<TTransaction, TTrading>>())
                        {
                            RemoveRunner(oldItem);
                        }

                        break;
                    case NotifyCollectionChangedAction.Reset:
                        foreach (var runner in _tradings)
                        {
                            OnRunnerChanged(runner);
                            runner.Changed -= OnRunnerChanged;
                        }

                        _tradings.Clear();
                        ReloadRunners();
                        break;
                }
            });
        }

        private void ReloadRunners()
        {
            var runners = AlgorithmManager.Runners
                .OfType<IAlgorithmRunner<TTransaction, TTrading>>()
                .ToArray();
            foreach (var runner in runners)
            {
                AddRunner(runner);
            }
        }
        private void RemoveRunner(IAlgorithmRunner<TTransaction, TTrading> runner)
        {
            OnRunnerChanged(runner);
            runner.Changed -= OnRunnerChanged;
            _tradings.Remove(runner);
        }

        private void AddRunner(IAlgorithmRunner<TTransaction, TTrading> runner)
        {
            OnRunnerChanged(runner);
            runner.Changed += OnRunnerChanged;
            _tradings.Add(runner);
        }

        public void Dispose()
        {
            ((INotifyCollectionChanged)AlgorithmManager.Runners).CollectionChanged -= OnRunnersChanged;
        }
        
        protected virtual void OnRunnerChanged(IAlgorithmRunner<TTransaction, TTrading> runner)
        {
            Tradings.FilterWhenNeeded(runner, Filter);
        }
    }
}