namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Components;

    public abstract partial class ViewBase<TTransaction, TTrading, TRunner> : ComponentBase, IDisposable
        where TTransaction: TransactionBase<TTrading>
        where TTrading : TradingBase, new()
        where TRunner : IAlgorithmRunner<TTransaction, TTrading>
    {
        [Inject] protected AlgorithmManager AlgorithmManager { get; init; }
        [Inject] protected NavigationManager NavigationManager { get; init; }

        [Parameter] public string Id { get; set; }

        protected TTrading Model;
        
        protected TRunner CurrentRunner;

        protected abstract string GetListUrl();
        
        protected abstract TTrading GetTrading(Guid id);


        protected override void OnInitialized()
        {
            OnLocationChangedInternal();
            NavigationManager.LocationChanged += (_, _) => OnLocationChangedInternal();
        }

        private void OnLocationChangedInternal()
        {
            if (!Guid.TryParse(Id, out var id))
            {
                NavigationManager.NavigateTo("/");
            }
            Model = GetTrading(id);
            if (CurrentRunner != null) CurrentRunner.Changed -= OnRunnerChangedInternal;
            CurrentRunner = AlgorithmManager.Runners
                .OfType<TRunner>()
                .SingleOrDefault(r => r.Context.Trading.Id == id);
            if (CurrentRunner != null) CurrentRunner.Changed += OnRunnerChangedInternal;

            OnLocationChanged();
            StateHasChanged();
        }
        protected virtual void OnLocationChanged() { }
        
        public void Dispose()
        {
            if (CurrentRunner != null)
            {
                CurrentRunner.Changed -= OnRunnerChangedInternal;
            }
        }

        private void OnRunnerChangedInternal(IAlgorithmRunner<TTransaction, TTrading> runner)
        {
            OnRunnerChanged();
            InvokeAsync(StateHasChanged);
        }
        protected virtual void OnRunnerChanged() { }
    }
}