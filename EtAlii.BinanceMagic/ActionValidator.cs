namespace EtAlii.BinanceMagic
{
    using System.Linq;
    using System.Threading;
    using Binance.Net;
    using Binance.Net.Objects.Spot.MarketData;

    public class ActionValidator
    {
        private readonly Settings _settings;
        private readonly Program _program;
        private readonly BinanceClient _client;

        public ActionValidator(Settings settings, Program program, BinanceClient client)
        {
            _settings = settings;
            _program = program;
            _client = client;
        }

        public void Validate(Action sellAction, string type, BinanceExchangeInfo exchangeInfo, CancellationToken cancellationToken)
        {
            var symbol = exchangeInfo.Symbols.Single(s => s.BaseAsset == sellAction.Coin && s.QuoteAsset == _settings.ReferenceCoin);

            var priceResult = _client.Spot.Market.GetPrice(symbol.Name, cancellationToken);
            var price = priceResult.Data.Price;
            if (symbol.PriceFilter is var priceFilter)
            {
                if (sellAction.TargetPrice <= priceFilter!.MinPrice)
                {
                    var message = $"{type} action target price {sellAction.TargetPrice} is below price filter minimum of {priceFilter.MinPrice}";
                    _program.HandleFail(message);
                }
                if (sellAction.TargetPrice >= priceFilter.MaxPrice)
                {
                    var message = $"{type} action target price {sellAction.TargetPrice} is above price filter maximum of {priceFilter.MaxPrice}";
                    _program.HandleFail(message);
                }
            }

            if (symbol.PricePercentFilter is var percentPriceFilter)
            {
                var min = percentPriceFilter!.MultiplierDown * price;
                var max = percentPriceFilter.MultiplierUp * price;
                if (sellAction.TargetPrice <= min)
                {
                    var message = $"{type} action target price {sellAction.TargetPrice} is below price percent filter minimum of {min}";
                    _program.HandleFail(message);
                }
                if (sellAction.TargetPrice >= max)
                {
                    var message = $"{type} action target price {sellAction.TargetPrice} is above price percent filter maximum of {max}";
                    _program.HandleFail(message);
                }
            }
            
            if (symbol.MinNotionalFilter is var minNotionalFilter)
            {
                var notionalPrice = sellAction.Quantity * sellAction.TargetPrice;
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
                if (sellAction.Quantity <= min)
                {
                    var message = $"{type} action lot quantity {sellAction.Quantity} is below minimum lot size of {min}";
                    _program.HandleFail(message);
                }
                if (sellAction.Quantity >= max)
                {
                    var message = $"{type} action lot quantity {sellAction.Quantity} is above minimum lot size of {max}";
                    _program.HandleFail(message);
                }
            }
        }
    }
}