namespace EtAlii.BinanceMagic.Service.Shared
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
    using Microsoft.EntityFrameworkCore;

    public partial class NavMenu
    {
        private bool _uiElementsVisible = true;

        private ObservableCollection<CircularTrading> CircularTradings { get; } = new();
        private ObservableCollection<SimpleTrading> SimpleTradings { get; } = new();
        private ObservableCollection<SurfingTrading> SurfingTradings { get; } = new();
        private ObservableCollection<ExperimentalTrading> ExperimentalTradings { get; } = new();

        protected override async Task OnInitializedAsync()
        {
            var data = new DataContext();

            var simpleTradings = await data.SimpleTradings.ToArrayAsync();
            foreach (var simpleTrading in simpleTradings)
            {
                SimpleTradings.Add(simpleTrading);
            }

            var circularTradings = await data.CircularTradings.ToArrayAsync();
            foreach (var circularTrading in circularTradings)
            {
                CircularTradings.Add(circularTrading);
            }
            
            var surfingTradings = await data.SurfingTradings.ToArrayAsync();
            foreach (var surfingTrading in surfingTradings)
            {
                SurfingTradings.Add(surfingTrading);
            }

            var experimentalTradings = await data.ExperimentalTradings.ToArrayAsync();
            foreach (var experimentalTrading in experimentalTradings)
            {
                ExperimentalTradings.Add(experimentalTrading);
            }

        }
    }
}