namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Components;

    public abstract class ViewBase<TTrading, TRunner> : ComponentBase, IDisposable
        where TTrading : TradingBase, new()
        where TRunner : IAlgorithmRunner<TTrading>
    {
        [Inject] AlgorithmManager AlgorithmManager { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }

        [Parameter] public string Id { get; set; }

        protected TTrading Model;
        
        protected MarkupString CurrentRunnerLog => (MarkupString) CurrentRunner.Log;

        protected TRunner CurrentRunner;

        protected abstract TTrading GetTrading(Guid id);


        protected override void OnInitialized()
        {
            OnLocationChangedInternal();
            NavigationManager.LocationChanged += (_, _) => OnLocationChangedInternal();
        }

        private void OnLocationChangedInternal()
        {
            var id = Guid.Parse(Id);
            Model = GetTrading(id);
            if (CurrentRunner != null)
            {
                CurrentRunner.Changed -= OnRunnerChangedInternal;
            }
            CurrentRunner = AlgorithmManager.Runners
                .OfType<TRunner>()
                .Single(r => r.Trading.Id == id);
            CurrentRunner.Changed += OnRunnerChangedInternal;

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

        protected virtual void OnRunnerChangedInternal()
        {
            OnRunnerChanged();
            InvokeAsync(StateHasChanged);
        }
        protected virtual void OnRunnerChanged() { }
    }
}