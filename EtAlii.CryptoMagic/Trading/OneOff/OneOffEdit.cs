namespace EtAlii.CryptoMagic
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public partial class OneOffEdit 
    {
        private decimal? PurchaseQuoteQuantity
        {
            get => Model.PurchaseQuoteQuantity;
            set => Model.PurchaseQuoteQuantity = value ?? 50;
        }

        private decimal TargetPercentageIncrease
        {
            get => Model.TargetPercentageIncrease;
            set => Model.TargetPercentageIncrease = value;
        }
        
        protected override OneOffTrading GetTrading(Guid id)
        {
            using var data = new DataContext();
            return data.OneOffTradings.Single(t => t.Id == id);
        }

        protected override string GetNavigationUrl(Guid id) => $"/one-off/list";
        
        protected override async Task<bool> CanSubmit()
        {
            (bool success, string error) = await CanStart();

            _error = error;
            if (!success)
            {
                ShowFailureDialog();
            }
            
            return success;
        }
    }
}