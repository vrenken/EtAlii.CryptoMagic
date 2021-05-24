namespace EtAlii.BinanceMagic.Service
{
    using System.Linq;
    using System.Threading;
    using Binance.Net;
    using Binance.Net.Objects.Spot.MarketData;

    public class ActionValidator : IActionValidator
    {
        public bool TryValidate<TAction>(BinanceClient client, TAction action, string type, string referenceCoin, BinanceExchangeInfo exchangeInfo, CancellationToken cancellationToken, out TAction outAction, out string error)
            where TAction : TradeAction
        {
            var symbol = exchangeInfo.Symbols.Single(s => s.BaseAsset == action.Symbol && s.QuoteAsset == referenceCoin);

            var priceResult = client.Spot.Market.GetPrice(symbol.Name, cancellationToken);
            var price = priceResult.Data.Price;
            if (symbol.PriceFilter is var priceFilter)
            {
                if (action.Price <= priceFilter!.MinPrice)
                {
                    error = $"{type} action target price {action.Price} is below price filter minimum of {priceFilter.MinPrice}";
                    outAction = null;
                    return false;
                }
                if (action.Price >= priceFilter.MaxPrice)
                {
                    error = $"{type} action target price {action.Price} is above price filter maximum of {priceFilter.MaxPrice}";
                    outAction = null;
                    return false;
                }
            }

            if (symbol.PricePercentFilter is var percentPriceFilter)
            {
                var min = percentPriceFilter!.MultiplierDown * price;
                var max = percentPriceFilter.MultiplierUp * price;
                if (action.Price <= min)
                {
                    error = $"{type} action target price {action.Price} is below price percent filter minimum of {min}";
                    outAction = null;
                    return false;
                }
                if (action.Price >= max)
                {
                    error = $"{type} action target price {action.Price} is above price percent filter maximum of {max}";
                    outAction = null;
                    return false;
                }
            }
            
            if (symbol.MinNotionalFilter is var minNotionalFilter)
            {
                var notionalPrice = action.Quantity * action.Price;
                if (notionalPrice <= minNotionalFilter!.MinNotional)
                {
                    error = $"{type} action notional price {notionalPrice} is below notional price filter of {minNotionalFilter.MinNotional}";
                    outAction = null;
                    return false;
                }
            }
            if (symbol.LotSizeFilter is var lotSizeFilter)
            {
                var min = lotSizeFilter!.MinQuantity;
                var max = lotSizeFilter.MaxQuantity;
                if (action.Quantity <= min)
                {
                    error = $"{type} action lot quantity {action.Quantity} is below minimum lot size of {min}";
                    outAction = null;
                    return false;
                }
                if (action.Quantity >= max)
                {
                    error = $"{type} action lot quantity {action.Quantity} is above minimum lot size of {max}";
                    outAction = null;
                    return false;
                }
            }
            
            action = action with { QuotedQuantity = decimal.Round(action.QuotedQuantity, symbol.QuoteAssetPrecision) };
            action = action with { Quantity = decimal.Round(action.Quantity, symbol.BaseAssetPrecision) };

            outAction = action;
            error = null;
            return true;
        }
    }
}