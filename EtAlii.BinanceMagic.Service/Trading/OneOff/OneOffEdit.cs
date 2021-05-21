namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Linq;
    using System.Threading;

    public partial class OneOffEdit 
    {
        private decimal? PurchasePrice
        {
            get => Model.PurchasePrice;
            set => Model.PurchasePrice = value ?? 0;
        }
        
        private bool CanFetchPrice { get; set; }

        private decimal? QuoteQuantity
        {
            get => Model.QuoteQuantity;
            set => Model.QuoteQuantity = value ?? 0;
        }
        
        protected override OneOffTrading GetTrading(Guid id)
        {
            using var data = new DataContext();
            return data.OneOffTradings.Single(t => t.Id == id);
        }

        protected override string GetNavigationUrl(Guid id) => $"/one-off/view/{Model.Id}";

        private void OnFetchPriceClicked()
        {
            if (ApplicationContext.LiveClient.TryGetPrice(Model.Symbol, Model.ReferenceSymbol, CancellationToken.None, out var price, out _))
            {
                PurchasePrice = price;
            }
            else
            {
                PurchasePrice = null;
            }
        }
        
        private void OnFetchQuoteQuantityClicked()
        {
            if (ApplicationContext.LiveClient.TryGetPrice(Model.Symbol, Model.ReferenceSymbol, CancellationToken.None, out var price, out _))
            {
                PurchasePrice = price;
            }
            else
            {
                PurchasePrice = null;
            }
        }

        private void OnHasValidSymbolChanged(bool hasValidSymbol) => CanFetchPrice = hasValidSymbol;
    }
}