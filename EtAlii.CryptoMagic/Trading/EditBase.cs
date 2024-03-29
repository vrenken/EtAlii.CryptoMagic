﻿namespace EtAlii.CryptoMagic
{
    using System;
    using System.Threading.Tasks;
    using Blazorise;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Forms;

    public abstract partial class EditBase<TTrading> : ComponentBase
        where TTrading : TradingBase, new()
    {
        [Inject] AlgorithmManager AlgorithmManager { get; init; }
        [Inject] protected NavigationManager NavigationManager { get; init; }
        
        [Inject] protected ApplicationContext ApplicationContext { get; init; }

        [Parameter] public string Id { get; set; }

        protected Validations Validations;
        protected TTrading Model;
        protected EditContext EditContext;

        protected abstract TTrading GetTrading(Guid id);

        protected virtual TTrading CreateTrading()
        {
            return new ()
            {
                Start = DateTime.Now,
                ReferenceSymbol = ApplicationContext.ReferenceSymbol
            };        
        }

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
                Model = CreateTrading();
            }

            EditContext = new EditContext(Model);
        }

        protected virtual async Task Submit()
        {
            if (await Validations.ValidateAll())
            {
                if (await CanSubmit())
                {
                    AlgorithmManager.Update(Model);

                    var navigationUrl = GetNavigationUrl(Model.Id);
                
                    NavigationManager.NavigateTo(navigationUrl);
                }
            }
        }

        protected virtual Task<bool> CanSubmit() => Task.FromResult(true);
    }
}