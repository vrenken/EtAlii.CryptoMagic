namespace EtAlii.BinanceMagic
{
    using System.Linq;
    using System.Threading;
    using Binance.Net;
    using Binance.Net.Objects.Spot.MarketData;

    public class ActionValidator : IActionValidator
    {
        public bool TryValidate<TAction>(BinanceClient client, TAction action, string type, string referenceCoin, BinanceExchangeInfo exchangeInfo, CancellationToken cancellationToken, out TAction outAction)
            where TAction : Action
        {
            var symbol = exchangeInfo.Symbols.Single(s => s.BaseAsset == action.Coin && s.QuoteAsset == referenceCoin);

            var priceResult = client.Spot.Market.GetPrice(symbol.Name, cancellationToken);
            var price = priceResult.Data.Price;
            if (symbol.PriceFilter is var priceFilter)
            {
                if (action.UnitPrice <= priceFilter!.MinPrice)
                {
                    var message = $"{type} action target price {action.UnitPrice} is below price filter minimum of {priceFilter.MinPrice}";
                    ConsoleOutput.WriteNegative(message);
                    outAction = null;
                    return false;
                }
                if (action.UnitPrice >= priceFilter.MaxPrice)
                {
                    var message = $"{type} action target price {action.UnitPrice} is above price filter maximum of {priceFilter.MaxPrice}";
                    ConsoleOutput.WriteNegative(message);
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
                    var message = $"{type} action target price {action.UnitPrice} is below price percent filter minimum of {min}";
                    ConsoleOutput.WriteNegative(message);
                    outAction = null;
                    return false;
                }
                if (action.UnitPrice >= max)
                {
                    var message = $"{type} action target price {action.UnitPrice} is above price percent filter maximum of {max}";
                    ConsoleOutput.WriteNegative(message);
                    outAction = null;
                    return false;
                }
            }
            
            if (symbol.MinNotionalFilter is var minNotionalFilter)
            {
                var notionalPrice = action.Quantity * action.UnitPrice;
                if (notionalPrice <= minNotionalFilter!.MinNotional)
                {
                    var message = $"{type} action notional price {notionalPrice} is below notional price filter of {minNotionalFilter.MinNotional}";
                    ConsoleOutput.WriteNegative(message);
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
                    var message = $"{type} action lot quantity {action.Quantity} is below minimum lot size of {min}";
                    ConsoleOutput.WriteNegative(message);
                    outAction = null;
                    return false;
                }
                if (action.Quantity >= max)
                {
                    var message = $"{type} action lot quantity {action.Quantity} is above minimum lot size of {max}";
                    ConsoleOutput.WriteNegative(message);
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