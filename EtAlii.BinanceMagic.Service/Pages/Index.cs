namespace EtAlii.BinanceMagic.Service.Pages
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using EtAlii.BinanceMagic.Service.Data;

    public partial class Index
    {
        private int _currentCount;

        private WeatherForecast[] _forecasts;

        private readonly Experiment[] _experiments;
        private Experiment _currentExperiment;

        public Index()
        {
            _experiments = new[]
            {
                new Experiment { Name = "Circular: BTC-BNB" },
                new Experiment { Name = "Circular: LTC-XMR" },
                new Experiment { Name = "Surfing: BTC-BNB-XMR" },
            };
            _currentExperiment = _experiments.First();
            UpdateExperiments();
        }
        
        private void SelectExperiment(string experimentName)
        {
            InvokeAsync(() =>
            {
                _currentExperiment = _experiments.SingleOrDefault(e => e.Name == experimentName);
                
                UpdateExperiments();
                StateHasChanged();
            });
        }

        private void UpdateExperiments()
        {
            foreach (var experiment in _experiments)
            {
                experiment.IsActive = _currentExperiment == experiment;
            }
        }
        
        protected override async Task OnInitializedAsync()
        {
            _forecasts = await _forecastService.GetForecastAsync(DateTime.Now);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _algorithmRunnerService.Tick += () =>
            {
                InvokeAsync(() =>
                {
                    _currentCount += 1;
                    StateHasChanged();
                });
            };
        }
    }
}