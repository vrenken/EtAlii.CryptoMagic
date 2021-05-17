namespace EtAlii.BinanceMagic.Service.Trading
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
        
        protected readonly Experiment[] Experiments;
        protected Experiment CurrentExperiment;

        protected ViewBase()
        {
            Experiments = new[]
            {
                new Experiment { Name = "Circular: BTC-BNB", Steps = new []{ new ExperimentStep(), new ExperimentStep(), } },
                new Experiment { Name = "Circular: LTC-XMR" },
                new Experiment { Name = "Surfing: BTC-BNB-XMR" },
            };
            CurrentExperiment = Experiments.First();
            UpdateExperiments();
        }

        protected abstract TTrading GetTrading(Guid id);


        protected override void OnInitialized()
        {
            var id = Guid.Parse(Id);
            Model = GetTrading(id);

            NavigationManager.LocationChanged += (_, _) =>
            {
                id = Guid.Parse(Id);
                Model = GetTrading(id);
                StateHasChanged();
            };
        }

        protected void SelectExperiment(string experimentName)
        {
            InvokeAsync(() =>
            {
                CurrentExperiment = Experiments.SingleOrDefault(e => e.Name == experimentName);
                
                UpdateExperiments();
                StateHasChanged();
            });
        }

        private void UpdateExperiments()
        {
            foreach (var experiment in Experiments)
            {
                experiment.IsActive = CurrentExperiment == experiment;
            }
        }
    }
}