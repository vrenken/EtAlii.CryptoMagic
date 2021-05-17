namespace EtAlii.BinanceMagic.Service.Trading
{
    using System;
    using Blazorise;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Forms;

    public abstract class EditBase<TTrading> : ComponentBase
        where TTrading : TradingBase, new()
    {
        [Inject] AlgorithmManager AlgorithmManager { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }

        [Parameter] public string Id { get; set; }

        protected Validations Validations;
        protected TTrading Model;
        protected EditContext EditContext;

        protected abstract TTrading GetTrading(Guid id);

        protected abstract string GetNavigationUrl(Guid id);

        protected override void OnInitialized()
        {
            if (Id != null)
            {
                var id = Guid.Parse(Id);
                Model = GetTrading(id);
            }
            else
            {
                Model = new TTrading();
            }

            EditContext = new EditContext(Model);
        }

        protected void Submit()
        {
            if (Validations.ValidateAll())
            {
                AlgorithmManager.Update(Model);

                var navigationUrl = GetNavigationUrl(Model.Id);
                
                NavigationManager.NavigateTo(navigationUrl);
            }
        }
    }
}