namespace EtAlii.BinanceMagic.Service
{
    using System;
    using Microsoft.AspNetCore.Components;

    public partial class CircularListItem : IDisposable
    {
        [Parameter] public IAlgorithmContext<CircularTransaction, CircularTrading> Context { get; set; }
        [Inject] NavigationManager NavigationManager { get; init; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Context.Changed += OnContextChanged;
        }

        private void OnContextChanged(AlgorithmChange change) => InvokeAsync(StateHasChanged);

        private void OnTradingSelected(Guid tradingId)
        {
            var navigationUrl = $"/circular/view/{tradingId}";
            NavigationManager.NavigateTo(navigationUrl);
        }

        public void Dispose()
        {
            Context.Changed -= OnContextChanged;
        }
    }
}