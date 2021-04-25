namespace EtAlii.BinanceMagic
{
    using System.Linq;
    using System.Threading;
    using Binance.Net;
    using Binance.Net.Objects.Spot.MarketData;

    public class ActionValidator
    {
        private readonly Program _program;
        private readonly BinanceClient _client;

        public ActionValidator(Program program, BinanceClient client)
        {
            _program = program;
            _client = client;
        }

        public TAction Validate<TAction>(TAction action, string type, string referenceCoin, BinanceExchangeInfo exchangeInfo, CancellationToken cancellationToken)
            where TAction : Action
        {
            var symbol = exchangeInfo.Symbols.Single(s => s.BaseAsset == action.Coin && s.QuoteAsset == referenceCoin);

            var priceResult = _client.Spot.Market.GetPrice(symbol.Name, cancellationToken);
            var price = priceResult.Data.Price;
            if (symbol.PriceFilter is var priceFilter)
            {
                if (action.UnitPrice <= priceFilter!.MinPrice)
                {
                    var message = $"{type} action target price {action.UnitPrice} is below price filter minimum of {priceFilter.MinPrice}";
                    _program.HandleFail(message);
                }
                if (action.UnitPrice >= priceFilter.MaxPrice)
                {
                    var message = $"{type} action target price {action.UnitPrice} is above price filter maximum of {priceFilter.MaxPrice}";
                    _program.HandleFail(message);
                }
            }

            if (symbol.PricePercentFilter is var percentPriceFilter)
            {
                var min = percentPriceFilter!.MultiplierDown * price;
                var max = percentPriceFilter.MultiplierUp * price;
                if (action.UnitPrice <= min)
                {
                    var message = $"{type} action target price {action.UnitPrice} is below price percent filter minimum of {min}";
                    _program.HandleFail(message);
                }
                if (action.UnitPrice >= max)
                {
                    var message = $"{type} action target price {action.UnitPrice} is above price percent filter maximum of {max}";
                    _program.HandleFail(message);
                }
            }
            
            if (symbol.MinNotionalFilter is var minNotionalFilter)
            {
                var notionalPrice = action.Quantity * action.UnitPrice;
                if (notionalPrice <= minNotionalFilter!.MinNotional)
                {
                    var message = $"{type} action notional price {notionalPrice} is below notional price filter of {minNotionalFilter.MinNotional}";
                    _program.HandleFail(message);
                }
            }
            if (symbol.LotSizeFilter is var lotSizeFilter)
            {
                var min = lotSizeFilter!.MinQuantity;
                var max = lotSizeFilter.MaxQuantity;
                if (action.Quantity <= min)
                {
                    var message = $"{type} action lot quantity {action.Quantity} is below minimum lot size of {min}";
                    _program.HandleFail(message);
                }
                if (action.Quantity >= max)
                {
                    var message = $"{type} action lot quantity {action.Quantity} is above minimum lot size of {max}";
                    _program.HandleFail(message);
                }
            }
            
            
            action = action with { Price = decimal.Round(action.Price, symbol.QuoteAssetPrecision) };
            action = action with { Quantity = decimal.Round(action.Quantity, symbol.BaseAssetPrecision) };

            return action;
        }
    }
}