namespace EtAlii.BinanceMagic.Service
{
    using System.Linq;
    using System.Threading.Tasks;
    using Blazorise;
    using Blazorise.Components;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Forms;

    public class SymbolTextEdit : Autocomplete<SymbolDefinition, string>
    {
        [Inject] protected ApplicationContext ApplicationContext { get; init; }

        [Parameter] public bool HasValidSymbol { get; set; }
        [Parameter] public EventCallback<bool> HasValidSymbolChanged { get; set; }
        
        [CascadingParameter] public EditContext EditContext { get; protected set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Data = ApplicationContext.Symbols;

            TextField = item => item.ToString();
            ValueField = item => item.Name;

            SearchChanged = EventCallback.Factory.Create<string>(this, OnSearchChanged);

            Validator = ValidateSymbol;
        }

        private void OnSearchChanged(object text)
        {
            EditContext.Validate();
            
            HasValidSymbol = text == null;
            HasValidSymbolChanged.InvokeAsync(HasValidSymbol);
        }

        private void ValidateSymbol(ValidatorEventArgs e)
        {
            if (e.Value is not string symbol)
            {
                e.Status = ValidationStatus.Error;
                return;
            }

            if (!HasValidSymbol)
            {
                e.Status = ValidationStatus.Error;
                return;
            }

            if (Data.All(s => s.Name != symbol))
            {
                e.Status = ValidationStatus.Error;
                return;
            }
            
            e.Status = ValidationStatus.Success;
        }
    }
}