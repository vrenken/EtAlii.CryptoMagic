namespace EtAlii.CryptoMagic
{
    using System.Threading.Tasks;
    using Blazorise;
    using Microsoft.AspNetCore.Components;

    public abstract partial class EditBase<TTrading> 
    {
        protected bool IsInvalid { get; private set; } = true;
        protected EventCallback<bool> IsInvalidChanged { get; set; }

        private bool _isValidating;
        
        protected async Task OnValidationChanged(ValidationsStatusChangedEventArgs e)
        {
            if (!_isValidating)
            {
                _isValidating = true;
                IsInvalid = !Validations.ValidateAll();
                await IsInvalidChanged.InvokeAsync(IsInvalid);
                _isValidating = false;
            }
        }

        protected void IsBetweenInclusive(ValidatorEventArgs e, decimal min, decimal max)
        {
            if (e.Value is decimal d)
            {
                e.Status = min <= d && d <= max ? ValidationStatus.Success : ValidationStatus.Error;
            }
            else if (e.Value is int i)
            {
                e.Status = min <= i && i <= max ? ValidationStatus.Success : ValidationStatus.Error;
            }
            else
            {
                e.Status = ValidationStatus.Error;
            }
        }

        protected void HasQuantity(ValidatorEventArgs e)
        {
            if (e.Value is decimal d)
            {
                e.Status = d > 0 ? ValidationStatus.Success : ValidationStatus.Error;
            }
            else
            {
                e.Status = ValidationStatus.Error;
            }
        }
    }
}