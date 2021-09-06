namespace EtAlii.CryptoMagic.Service
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Blazorise;

    public partial class OneOffEdit 
    {
        private Modal _failureDialogRef;
        private string _error;

        private void ShowFailureDialog()
        {
            _failureDialogRef.Show();
        }

        private void FailureConfirmed()
        {
            _failureDialogRef.Hide();

            var navigationUrl = GetNavigationUrl(Model.Id);
            NavigationManager.NavigateTo(navigationUrl);
        }

        private async Task<(bool success, string error)> CanStart()
        {
            var actionValidator = new ActionValidator();
            var client = new Client(actionValidator)
            {
                PlaceTestOrders = Model.TradeMethod == TradeMethod.BinanceTest
            };

            await using var data = new DataContext();
            var binanceApiKey = data.Settings.Single(s => s.Key == SettingKey.BinanceApiKey).Value;
            var binanceSecretKey = data.Settings.Single(s => s.Key == SettingKey.BinanceSecretKey).Value;
            await client.Start(binanceApiKey, binanceSecretKey);

            bool success;
            decimal price;
            string error;
            (success, price, error) = await client.TryGetPrice(Model.Symbol, Model.ReferenceSymbol, CancellationToken.None);

            var buyAction = new BuyAction
            {
                Price = price,
                QuotedQuantity = Model.PurchaseQuoteQuantity,
                Quantity = 0m, // Not used during buying.
                Symbol = Model.Symbol,
                TransactionId = $"{Model.Symbol}_{Model.Start:yyyyMMdd_HHmmss}",
            };

            Symbol symbolsBought;
            (success, symbolsBought, error) = await client.TryBuySymbol(buyAction, Model.ReferenceSymbol, CancellationToken.None, () => DateTime.Now);

            if (success)
            {
                Model.PurchasePrice = symbolsBought.Price;
                Model.CurrentPrice = symbolsBought.Price;
                Model.PurchaseQuoteQuantity = symbolsBought.QuoteQuantity;
                Model.PurchaseSymbolQuantity = symbolsBought.Quantity;
                Model.FinalQuoteQuantity = symbolsBought.QuoteQuantity;
                Model.Name = $"{Model.PurchaseSymbolQuantity:0.000000} {Model.Symbol} ({Model.TargetPercentageIncrease}%)";
            }

            client.Stop();
            
            return (success, error);
        }
    }
}