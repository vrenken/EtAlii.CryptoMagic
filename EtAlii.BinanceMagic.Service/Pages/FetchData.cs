namespace EtAlii.BinanceMagic.Service.Pages
{
    using System;
    using System.Threading.Tasks;
    using EtAlii.BinanceMagic.Service.Data;

    public partial class FetchData
    {
        private WeatherForecast[] _forecasts;

        protected override async Task OnInitializedAsync()
        {
            _forecasts = await _forecastService.GetForecastAsync(DateTime.Now);
        }

    }
}