namespace EtAlii.BinanceMagic
{
    using System;
    using System.Linq;
    using System.Threading;
    using Binance.Net;
    using Binance.Net.Enums;
    using Binance.Net.Objects.Spot;
    using CryptoExchange.Net.Authentication;
    using CryptoExchange.Net.Objects;

    public class Client
    {
        private BinanceClient _client;
        //private BinanceSocketClient _socketClient;

        private readonly Settings _settings;
        private readonly Program _program;
        private ActionValidator _validator;

        public Client(Settings settings, Program program)
        {
            _settings = settings;
            _program = program;
        }

        public void Start()
        {
            ConsoleOutput.Write("Starting Binance client...");
            
            var options = new BinanceClientOptions
            {
                RateLimitingBehaviour = RateLimitingBehaviour.Wait,
                ApiCredentials = new ApiCredentials(_settings.ApiKey, _settings.SecretKey),
            };
            _client = new BinanceClient(options);

            // var socketOptions = new BinanceSocketClientOptions
            // {
            //     ApiCredentials = new ApiCredentials(_settings.ApiKey, _settings.SecretKey),
            // };
            //_socketClient = new BinanceSocketClient(socketOptions);
            //var result = _socketClient.Spot.SubscribeToSymbolMiniTickerUpdates(new[] {""}, b => b.);

            var startResult = _client.Spot.UserStream.StartUserStream();

            if (!startResult.Success)
            {
                var message = $"Failed to start user stream: {startResult.Error}";
                _program.HandleFail(message);
            }

            _validator = new ActionValidator(_settings, _program, _client);
            
            ConsoleOutput.Write("Starting Binance client: Done");
            
        }

        public decimal GetPrice(string coin, CancellationToken cancellationToken)
        {
            var coinComparedToReference = $"{coin}{_settings.ReferenceCoin}"; 
            var result = _client.Spot.Market.GetPrice(coinComparedToReference, cancellationToken);
            if (result.Error != null)
            {
                var message = $"Failure fetching price for {coin}: {result.Error}";
                _program.HandleFail(message);
            }

            return result.Data.Price;
        }

        public (decimal MakerFee, decimal TakerFee) GetTradeFees(string coin, CancellationToken cancellationToken)
        {
            var coinComparedToReference = $"{coin}{_settings.ReferenceCoin}"; 
            var result = _client.Spot.Market.GetTradeFee(coinComparedToReference, null, cancellationToken);
            if (result.Error != null)
            {
                var message = $"Failure fetching trade fees for {coin}: {result.Error}";
                _program.HandleFail(message);
            }

            var fees = result.Data.First();
            return (fees.MakerFee, fees.TakerFee);
        }

        public (decimal quantityToSell, decimal quantityToBuy) GetMinimalQuantity(string coinToSell, string coinToBuy, CancellationToken cancellationToken)
        {
            ConsoleOutput.Write("Fetching exchange info...");
            var exchangeResult = _client.Spot.System.GetExchangeInfo();
            if (!exchangeResult.Success)
            {
                var message = $"Failed to fetch exchange info: {exchangeResult.Error}";
                _program.HandleFail(message);
            }
            var exchangeInfo = exchangeResult.Data;

            var symbolToSell = exchangeInfo.Symbols.Single(s => s.BaseAsset == coinToSell && s.QuoteAsset == _settings.ReferenceCoin);

            var min = symbolToSell.PriceFilter!.MinPrice;
            var minPercent = symbolToSell.PricePercentFilter!.MultiplierDown;
            var notional = symbolToSell.MinNotionalFilter!.MinNotional;
            var quantityToSell = ((min * minPercent) / notional) * _settings.InitialPurchaseMinimalFactor;
            //var quantityToSell = symbolToSell.LotSizeFilter!.MinQuantity * _settings.InitialPurchaseMinimalFactor;

            var symbolToBuy = exchangeInfo.Symbols.Single(s => s.BaseAsset == coinToBuy && s.QuoteAsset == _settings.ReferenceCoin);

            var price = GetPrice(coinToBuy, cancellationToken);
            
            var quantityToBuy = (1 / price) * symbolToBuy.MinNotionalFilter!.MinNotional * _settings.InitialPurchaseMinimalFactor;

            return (quantityToSell, quantityToBuy);
        }
        
        public bool TryConvert(SellAction sellAction, BuyAction buyAction, CancellationToken cancellationToken, out Transaction transaction)
        {
            ConsoleOutput.Write("Fetching exchange info...");
            var exchangeResult = _client.Spot.System.GetExchangeInfo();
            if (!exchangeResult.Success)
            {
                var message = $"Failed to fetch exchange info: {exchangeResult.Error}";
                _program.HandleFail(message);
            }
            var exchangeInfo = exchangeResult.Data;

            _validator.Validate(sellAction, "Sell", exchangeInfo, cancellationToken);
            _validator.Validate(buyAction, "Buy", exchangeInfo, cancellationToken);
            
            var sellCoin = $"{sellAction.Coin}{_settings.ReferenceCoin}";
            var sellOrder = _client.Spot.Order.PlaceOrder(sellCoin, OrderSide.Sell, OrderType.Market,
                sellAction.Quantity, null, sellAction.TransactionId, null, null,
                null, null, OrderResponseType.Result, null, cancellationToken);

            if (sellOrder.Error != null)
            {
                var message = $"Failure placing sell order for {sellAction.Coin}: {sellOrder.Error}";
                _program.HandleFail(message);
            }
            if (sellOrder.Data.Status != OrderStatus.Filled)
            {
                var message = $"Failure placing sell order for {sellAction.Coin}: {sellOrder.Data.Status}";
                _program.HandleFail(message);
            }

            var buyQuantity = decimal.Round(buyAction.Quantity, 8); 
            var buyCoin = $"{buyAction.Coin}{_settings.ReferenceCoin}";
            var buyOrder = _client.Spot.Order.PlaceOrder(buyCoin, OrderSide.Buy, OrderType.Market,
                buyQuantity, null, buyAction.TransactionId, null, null,
                null, null, OrderResponseType.Result, null, cancellationToken);

            if (buyOrder.Error != null)
            {
                var message = $"Failure placing buy order for {buyAction.Coin}: {buyOrder.Error}";
                _program.HandleFail(message);
            }
            if (buyOrder.Data.Status != OrderStatus.Filled)
            {
                var message = $"Failure placing buy order for {buyAction.Coin}: {buyOrder.Data.Status}";
                _program.HandleFail(message);
            }

            transaction = new Transaction
            {
                From = new CoinSnapshot
                {
                    Coin = sellAction.Coin,
                    Price = sellOrder.Data.Price,
                    Quantity = sellOrder.Data.Quantity
                },
                To = new CoinSnapshot
                {
                    Coin = buyAction.Coin,
                    Price = buyOrder.Data.Price,
                    Quantity = buyOrder.Data.Quantity
                },
                Moment = DateTime.Now,
                TotalProfit =  (buyOrder.Data.Price * buyOrder.Data.Quantity) - (sellOrder.Data.Price * sellOrder.Data.Quantity) 
            };
            return true;
        }

        public void Stop()
        {
            ConsoleOutput.Write("Stopping Binance client...");
            _client.Dispose();
            ConsoleOutput.Write("Stopping Binance client: Done");
        }
    }
}