namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Components;

    public abstract class ViewBase<TTrading, TRunner> : ComponentBase, IDisposable
        where TTrading : TradingBase, new()
        where TRunner : IAlgorithmRunner
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
            var id = Guid.Parse(Id);
            Model = GetTrading(id);
            CurrentRunner = AlgorithmManager.Runners
                .OfType<TRunner>()
                .Single(r => r.Trading.Id == id);
            CurrentRunner.Changed += OnRunnerChanged;
            
            NavigationManager.LocationChanged += (_, _) =>
            {
                id = Guid.Parse(Id);
                Model = GetTrading(id);
                if (CurrentRunner != null)
                {
                    CurrentRunner.Changed -= OnRunnerChanged;
                }
                CurrentRunner = AlgorithmManager.Runners
                    .OfType<TRunner>()
                    .Single(r => r.Trading.Id == id);
                CurrentRunner.Changed += OnRunnerChanged;
                StateHasChanged();
            };
        }

        public void Dispose()
        {
            if (CurrentRunner != null)
            {
                CurrentRunner.Changed -= OnRunnerChanged;
            }
        }

        protected virtual void OnRunnerChanged() => InvokeAsync(StateHasChanged);
    }
}