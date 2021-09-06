namespace EtAlii.CryptoMagic.Service
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Binance.Net;
    using Binance.Net.Objects.Spot.MarketData;

    public class ActionValidator : IActionValidator
    {
        public async Task<(bool, TAction, string)> TryValidate<TAction>(BinanceClient client, TAction action, string type, string referenceCoin, BinanceExchangeInfo exchangeInfo, CancellationToken cancellationToken)
            where TAction : TradeAction
        {
            var symbol = exchangeInfo.Symbols.Single(s => s.BaseAsset == action.Symbol && s.QuoteAsset == referenceCoin);

            var priceResult = await client.Spot.Market.GetPriceAsync(symbol.Name, cancellationToken);
            var price = priceResult.Data.Price;
            if (symbol.PriceFilter is var priceFilter)
            {
                if (action.Price <= priceFilter!.MinPrice)
                {
                    var error = $"{type} action target price {action.Price} is below price filter minimum of {priceFilter.MinPrice}";
                    return (false, null, error);
                }
                if (action.Price >= priceFilter.MaxPrice)
                {
                    var error = $"{type} action target price {action.Price} is above price filter maximum of {priceFilter.MaxPrice}";
                    return (false, null, error);
                }
            }

            if (symbol.PricePercentFilter is var percentPriceFilter)
            {
                var min = percentPriceFilter!.MultiplierDown * price;
                var max = percentPriceFilter.MultiplierUp * price;
                if (action.Price <= min)
                {
                    var error = $"{type} action target price {action.Price} is below price percent filter minimum of {min}";
                    return (false, null, error);
                }
                if (action.Price >= max)
                {
                    var error = $"{type} action target price {action.Price} is above price percent filter maximum of {max}";
                    return (false, null, error);
                }
            }
            
            if (symbol.MinNotionalFilter is var minNotionalFilter)
            {
                var notionalPrice = action.Quantity * action.Price;
                if (notionalPrice <= minNotionalFilter!.MinNotional)
                {
                    var error = $"{type} action notional price {notionalPrice} is below notional price filter of {minNotionalFilter.MinNotional}";
                    return (false, null, error);
                }
            }
            if (symbol.LotSizeFilter is var lotSizeFilter)
            {
                var min = lotSizeFilter!.MinQuantity;
                var max = lotSizeFilter.MaxQuantity;
                if (action.Quantity <= min)
                {
                    var error = $"{type} action lot quantity {action.Quantity} is below minimum lot size of {min}";
                    return (false, null, error);
                }
                if (action.Quantity >= max)
                {
                    var error = $"{type} action lot quantity {action.Quantity} is above minimum lot size of {max}";
                    return (false, null, error);
                }
            }
            
            action = action with { QuotedQuantity = decimal.Round(action.QuotedQuantity, symbol.QuoteAssetPrecision) };
            action = action with { Quantity = decimal.Round(action.Quantity, symbol.BaseAssetPrecision) };

            return (true, action, null);
        }
    }
}