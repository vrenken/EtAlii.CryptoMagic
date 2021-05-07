namespace EtAlii.BinanceMagic
{
    using System.Linq;
    using System.Threading;
    using Binance.Net;
    using Binance.Net.Objects.Spot.MarketData;
    using EtAlii.BinanceMagic.Circular;

    public class ActionValidator : IActionValidator
    {
        public bool TryValidate<TAction>(BinanceClient client, TAction action, string type, string referenceCoin, BinanceExchangeInfo exchangeInfo, TradeDetails details, CancellationToken cancellationToken, out TAction outAction)
            where TAction : Action
        {
            var symbol = exchangeInfo.Symbols.Single(s => s.BaseAsset == action.Coin && s.QuoteAsset == referenceCoin);

            var priceResult = client.Spot.Market.GetPrice(symbol.Name, cancellationToken);
            var price = priceResult.Data.Price;
            if (symbol.PriceFilter is var priceFilter)
            {
                if (action.UnitPrice <= priceFilter!.MinPrice)
                {
                    details.Result = $"{type} action target price {action.UnitPrice} is below price filter minimum of {priceFilter.MinPrice}";
                    outAction = null;
                    return false;
                }
                if (action.UnitPrice >= priceFilter.MaxPrice)
                {
                    details.Result = $"{type} action target price {action.UnitPrice} is above price filter maximum of {priceFilter.MaxPrice}";
                    outAction = null;
                    return false;
                }
            }

            if (symbol.PricePercentFilter is var percentPriceFilter)
            {
                var min = percentPriceFilter!.MultiplierDown * price;
                var max = percentPriceFilter.MultiplierUp * price;
                if (action.UnitPrice <= min)
                {
                    details.Result = $"{type} action target price {action.UnitPrice} is below price percent filter minimum of {min}";
                    outAction = null;
                    return false;
                }
                if (action.UnitPrice >= max)
                {
                    details.Result = $"{type} action target price {action.UnitPrice} is above price percent filter maximum of {max}";
                    outAction = null;
                    return false;
                }
            }
            
            if (symbol.MinNotionalFilter is var minNotionalFilter)
            {
                var notionalPrice = action.Quantity * action.UnitPrice;
                if (notionalPrice <= minNotionalFilter!.MinNotional)
                {
                    details.Result = $"{type} action notional price {notionalPrice} is below notional price filter of {minNotionalFilter.MinNotional}";
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
                    details.Result = $"{type} action lot quantity {action.Quantity} is below minimum lot size of {min}";
                    outAction = null;
                    return false;
                }
                if (action.Quantity >= max)
                {
                    details.Result = $"{type} action lot quantity {action.Quantity} is above minimum lot size of {max}";
                    outAction = null;
                    return false;
                }
            }
            
            action = action with { Price = decimal.Round(action.Price, symbol.QuoteAssetPrecision) };
            action = action with { Quantity = decimal.Round(action.Quantity, symbol.BaseAssetPrecision) };

            outAction = action;
            return true;
        }
    }
}