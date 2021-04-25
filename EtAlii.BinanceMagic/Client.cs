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
        private readonly ProgramSettings _settings;
        private readonly Program _program;
        private ActionValidator _validator;

        public Client(ProgramSettings settings, Program program)
        {
            _settings = settings;
            _program = program;
        }

        public void Start()
        {
            ConsoleOutput.Write("Starting client...");
            
            var options = new BinanceClientOptions
            {
                RateLimitingBehaviour = RateLimitingBehaviour.Wait,
                ApiCredentials = new ApiCredentials(_settings.ApiKey, _settings.SecretKey),
                TradeRulesBehaviour = TradeRulesBehaviour.AutoComply,
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

            _validator = new ActionValidator(_program, _client);
            
            ConsoleOutput.Write("Starting client: Done");
            
        }

        public decimal GetPrice(string coin, string referenceCoin, CancellationToken cancellationToken)
        {
            var coinComparedToReference = $"{coin}{referenceCoin}"; 
            var result = _client.Spot.Market.GetPrice(coinComparedToReference, cancellationToken);
            if (result.Error != null)
            {
                var message = $"Failure fetching price for {coin}: {result.Error}";
                _program.HandleFail(message);
            }

            return result.Data.Price;
        }

        public (decimal MakerFee, decimal TakerFee) GetTradeFees(string coin, string referenceCoin, CancellationToken cancellationToken)
        {
            var coinComparedToReference = $"{coin}{referenceCoin}"; 
            var result = _client.Spot.Market.GetTradeFee(coinComparedToReference, null, cancellationToken);
            if (result.Error != null)
            {
                var message = $"Failure fetching trade fees for {coin}: {result.Error}";
                _program.HandleFail(message);
            }

            var fees = result.Data.First();
            return (fees.MakerFee, fees.TakerFee);
        }

        public (decimal quantityToSell, decimal quantityToBuy) GetMinimalQuantity(string coinToSell, string coinToBuy, LoopSettings loopSettings, CancellationToken cancellationToken)
        {
            //ConsoleOutput.Write("Fetching exchange info...");
            var exchangeResult = _client.Spot.System.GetExchangeInfo();
            if (!exchangeResult.Success)
            {
                var message = $"Failed to fetch exchange info: {exchangeResult.Error}";
                _program.HandleFail(message);
            }
            var exchangeInfo = exchangeResult.Data;

            var symbolToSell = exchangeInfo.Symbols.Single(s => s.BaseAsset == coinToSell && s.QuoteAsset == loopSettings.ReferenceCoin);

            var min = symbolToSell.PriceFilter!.MinPrice;
            var minPercent = symbolToSell.PricePercentFilter!.MultiplierDown;
            var notional = symbolToSell.MinNotionalFilter!.MinNotional;
            var quantityToSell = ((min * minPercent) / notional) * loopSettings.InitialPurchaseMinimalFactor;
            //var quantityToSell = symbolToSell.LotSizeFilter!.MinQuantity * _settings.InitialPurchaseMinimalFactor;

            var symbolToBuy = exchangeInfo.Symbols.Single(s => s.BaseAsset == coinToBuy && s.QuoteAsset == loopSettings.ReferenceCoin);

            var price = GetPrice(coinToBuy, loopSettings.ReferenceCoin, cancellationToken);
            
            var quantityToBuy = (1 / price) * symbolToBuy.MinNotionalFilter!.MinNotional * loopSettings.InitialPurchaseMinimalFactor;

            return (quantityToSell, quantityToBuy);
        }
        
        public bool TryConvert(SellAction sellAction, BuyAction buyAction, string referenceCoin, CancellationToken cancellationToken, out Transaction transaction)
        {
            //ConsoleOutput.Write("Fetching exchange info...");
            var exchangeResult = _client.Spot.System.GetExchangeInfo();
            if (!exchangeResult.Success)
            {
                var message = $"Failed to fetch exchange info: {exchangeResult.Error}";
                _program.HandleFail(message);
            }
            var exchangeInfo = exchangeResult.Data;

            var orderResponseType = OrderResponseType.Full;
            var timeInForce = (TimeInForce?) null;// TimeInForce.FillOrKill;
            var orderType = OrderType.Market;
            
            sellAction = _validator.Validate(sellAction, "Sell", referenceCoin, exchangeInfo, cancellationToken);
            buyAction = _validator.Validate(buyAction, "Buy", referenceCoin, exchangeInfo, cancellationToken);
            
            var sellCoin = $"{sellAction.Coin}{referenceCoin}";

            // ReSharper disable ExpressionIsAlwaysNull
            var sellOrder = _settings.PlaceTestOrders
                ? _client.Spot.Order.PlaceTestOrder(sellCoin, OrderSide.Sell, orderType, null, sellAction.Price, sellAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken)
                : _client.Spot.Order.PlaceOrder(sellCoin, OrderSide.Sell, orderType, null, sellAction.Price, sellAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken);
            // ReSharper restore ExpressionIsAlwaysNull

            if (sellOrder.Error != null)
            {
                var message = $"Failure placing sell order for {sellAction.Coin}: {sellOrder.Error}";
                _program.HandleFail(message);
            }

            var isSold = _settings.PlaceTestOrders
                ? sellOrder.Data.Status == OrderStatus.New
                : sellOrder.Data.Status == OrderStatus.Filled;
            if (!isSold)
            {
                var message = $"Failure placing sell order for {sellAction.Coin}: {sellOrder.Data.Status}";
                _program.HandleFail(message);
            }

            var buyCoin = $"{buyAction.Coin}{referenceCoin}";

            // ReSharper disable ExpressionIsAlwaysNull
            var buyOrder = _settings.PlaceTestOrders
                ? _client.Spot.Order.PlaceTestOrder(buyCoin, OrderSide.Buy, orderType, null, buyAction.Price, buyAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken)
                : _client.Spot.Order.PlaceOrder(buyCoin, OrderSide.Buy, orderType, null, buyAction.Price, buyAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken);
            // ReSharper restore ExpressionIsAlwaysNull

            if (buyOrder.Error != null)
            {
                var message = $"Failure placing buy order for {buyAction.Coin}: {buyOrder.Error}";
                _program.HandleFail(message);
            }

            var isBought = _settings.PlaceTestOrders
                ? buyOrder.Data.Status == OrderStatus.New
                : buyOrder.Data.Status == OrderStatus.Filled;
            if (!isBought)
            {
                var message = $"Failure placing buy order for {buyAction.Coin}: {buyOrder.Data.Status}";
                _program.HandleFail(message);
            }

            transaction = new Transaction
            {
                From = new CoinSnapshot
                {
                    Coin = sellAction.Coin,
                    Price = sellOrder.Data.QuoteQuantityFilled,
                    Quantity = sellOrder.Data.QuantityFilled
                },
                To = new CoinSnapshot
                {
                    Coin = buyAction.Coin,
                    Price = buyOrder.Data.QuoteQuantityFilled,
                    Quantity = buyOrder.Data.QuantityFilled
                },
                Moment = DateTime.Now,
                TotalProfit =  sellOrder.Data.QuoteQuantityFilled - buyOrder.Data.QuoteQuantityFilled 
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