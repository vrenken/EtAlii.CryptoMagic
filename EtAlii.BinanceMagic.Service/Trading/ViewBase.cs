namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Components;

    public abstract class ViewBase<TTrading> : ComponentBase
        where TTrading : TradingBase, new()
    {
        [Inject] AlgorithmManager AlgorithmManager { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }

        [Parameter] public string Id { get; set; }

        protected TTrading Model;
        
        protected MarkupString CurrentRunnerLog => (MarkupString) CurrentRunner.Log;

        protected IAlgorithmRunner CurrentRunner;

        protected abstract TTrading GetTrading(Guid id);


        protected override void OnInitialized()
        {
            var id = Guid.Parse(Id);
            Model = GetTrading(id);
            CurrentRunner = AlgorithmManager.Runners.Single(r => r.Trading.Id == id);
            
            NavigationManager.LocationChanged += (_, _) =>
            {
                id = Guid.Parse(Id);
                Model = GetTrading(id);
                CurrentRunner = AlgorithmManager.Runners.Single(r => r.Trading.Id == id);
                StateHasChanged();
            };
        }
    }
}