namespace EtAlii.BinanceMagic
{
    using System;
    using System.Linq;
    using System.Threading;
    using Binance.Net;
    using Binance.Net.Enums;
    using Binance.Net.Objects.Spot;
    using Binance.Net.Objects.Spot.MarketData;
    using Binance.Net.Objects.Spot.SpotData;
    using CryptoExchange.Net.Authentication;
    using CryptoExchange.Net.Objects;

    public class Client : IClient
    {
        private BinanceClient _client;
        private readonly ProgramSettings _settings;
        private readonly IProgram _program;
        private readonly IActionValidator _validator;
        private readonly IOutput _output;

        public bool PlaceTestOrders { get; init; }

        public Client(ProgramSettings settings, IProgram program, IActionValidator actionValidator, IOutput output)
        {
            _settings = settings;
            _program = program;
            _validator = actionValidator;
            _output = output;
        }

        public void Start()
        {
            _output.WriteLine("Starting client...");
            
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
            
            _output.WriteLine("Starting client: Done");
        }

        public bool TryGetPrice(string coin, string referenceCoin, TradeDetails details, CancellationToken cancellationToken, out decimal price)
        {
            var coinComparedToReference = $"{coin}{referenceCoin}"; 
            var result = _client.Spot.Market.GetPrice(coinComparedToReference, cancellationToken);
            if (result.Error != null)
            {
                details.Result = $"Failure fetching price for {coin}: {result.Error}";
                price = decimal.Zero;
                return false;
            }

            price = result.Data.Price;
            return true;
        }

        public bool TryGetTrend(string coin, string referenceCoin, TradeDetails details, CancellationToken cancellationToken, out decimal trend)
        {
            var coinComparedToReference = $"{coin}{referenceCoin}"; 
            var result = _client.Spot.Market.GetKlines(coinComparedToReference, KlineInterval.FiveMinutes, limit:1 , ct: cancellationToken);
            if (result.Error != null)
            {
                details.Result = $"Failure fetching candlestick data for {coin}: {result.Error}";
                trend = 0m;
                return false;
            }

            var data = result.Data.Single();
            trend = data.Close - data.Open;
            return true;
        }
        
        public bool TryGetTradeFees(string coin, string referenceCoin, TradeDetails details, CancellationToken cancellationToken, out decimal makerFee, out decimal takerFee)
        {
            var coinComparedToReference = $"{coin}{referenceCoin}"; 
            var result = _client.Spot.Market.GetTradeFee(coinComparedToReference, null, cancellationToken);
            if (result.Error != null)
            {
                details.Result = $"Failure fetching trade fees for {coin}: {result.Error}";
                makerFee = 0m;
                takerFee = 0m;
                return false;
            }

            var fees = result.Data.First();
            makerFee = fees.MakerFee;
            takerFee = fees.TakerFee;
            return true;
        }
        
        public bool TryGetExchangeInfo(TradeDetails details, CancellationToken cancellationToken, out BinanceExchangeInfo exchangeInfo)
        {
            var exchangeResult = _client.Spot.System.GetExchangeInfo(cancellationToken);
            if (!exchangeResult.Success)
            {
                details.Result = $"Failed to fetch exchange info: {exchangeResult.Error}";
                exchangeInfo = null;
                return false;
            }
            exchangeInfo = exchangeResult.Data;
            return true;
        }
        
        public bool TryConvert(SellAction sellAction, BuyAction buyAction, string referenceCoin, TradeDetails details, CancellationToken cancellationToken, Func<DateTime> getNow, out Transaction transaction)
        {
            var exchangeResult = _client.Spot.System.GetExchangeInfo();
            if (!exchangeResult.Success)
            {
                details.Result = $"Failed to fetch exchange info: {exchangeResult.Error}";
                transaction = null;
                return false;
            }
            var exchangeInfo = exchangeResult.Data;

            var orderResponseType = OrderResponseType.Full;
            var timeInForce = (TimeInForce?) null;// TimeInForce.FillOrKill;
            var orderType = OrderType.Market;

            if (!_validator.TryValidate(_client, sellAction, "Sell", referenceCoin, exchangeInfo, details, cancellationToken, out sellAction))
            {
                transaction = null;
                return false;
            }
            if(!_validator.TryValidate(_client, buyAction, "Buy", referenceCoin, exchangeInfo, details, cancellationToken, out buyAction))
            {
                transaction = null;
                return false;
            }
            
            var sellCoin = $"{sellAction.Coin}{referenceCoin}";

            // ReSharper disable ExpressionIsAlwaysNull
            var sellOrder = PlaceTestOrders
                ? _client.Spot.Order.PlaceTestOrder(sellCoin, OrderSide.Sell, orderType, null, sellAction.Price, sellAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken)
                : _client.Spot.Order.PlaceOrder(sellCoin, OrderSide.Sell, orderType, null, sellAction.Price, sellAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken);
            // ReSharper restore ExpressionIsAlwaysNull

            if (sellOrder.Error != null)
            {
                details.Result = $"Failure placing sell order for {sellAction.Coin}: {sellOrder.Error}";
                transaction = null;
                return false;
            }

            var isSold = PlaceTestOrders
                ? sellOrder.Data.Status == OrderStatus.New
                : sellOrder.Data.Status == OrderStatus.Filled;
            if (!isSold)
            {
                details.Result = $"Failure placing sell order for {sellAction.Coin}: {sellOrder.Data.Status}";
                transaction = null;
                return false;
            }
            
            var buyCoin = $"{buyAction.Coin}{referenceCoin}";

            // ReSharper disable ExpressionIsAlwaysNull
            var buyOrder = PlaceTestOrders
                ? _client.Spot.Order.PlaceTestOrder(buyCoin, OrderSide.Buy, orderType, null, buyAction.Price, buyAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken)
                : _client.Spot.Order.PlaceOrder(buyCoin, OrderSide.Buy, orderType, null, buyAction.Price, buyAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken);
            // ReSharper restore ExpressionIsAlwaysNull

            if (buyOrder.Error != null)
            {
                details.Result = $"Failure placing buy order for {buyAction.Coin}: {buyOrder.Error}";
                RollbackOrder(sellOrder.Data, cancellationToken);
                transaction = null;
                return false;
            }

            var isBought = PlaceTestOrders
                ? buyOrder.Data.Status == OrderStatus.New
                : buyOrder.Data.Status == OrderStatus.Filled;
            if (!isBought)
            {
                details.Result = $"Failure placing buy order for {buyAction.Coin}: {buyOrder.Data.Status}";
                RollbackOrder(sellOrder.Data, cancellationToken);
                transaction = null;
                return false;
            }

            transaction = new Transaction
            {
                From = new Coin
                {
                    Symbol = sellAction.Coin,
                    Price = sellOrder.Data.QuoteQuantityFilled,
                    Quantity = sellOrder.Data.QuantityFilled
                },
                To = new Coin
                {
                    Symbol = buyAction.Coin,
                    Price = buyOrder.Data.QuoteQuantityFilled,
                    Quantity = buyOrder.Data.QuantityFilled
                },
                Moment = getNow(),
                TotalProfit =  sellOrder.Data.QuoteQuantityFilled - buyOrder.Data.QuoteQuantityFilled 
            };
            return true;
        }

        private void RollbackOrder(BinancePlacedOrder order, CancellationToken cancellationToken)
        {
            var cancelOrder = _client.Spot.Order.CancelOrder(order.Symbol, order.OrderId, order.ClientOrderId, ct: cancellationToken);
            if (cancelOrder.Error != null)
            {
                var message = $"Failure cancelling order for {order.ClientOrderId}: {cancelOrder.Error}";
                _program.HandleFail(message);
            }
        }
        
        public void Stop()
        {
            _output.WriteLine("Stopping client...");
            _client.Dispose();
            _output.WriteLine("Stopping client: Done");
        }
    }
}