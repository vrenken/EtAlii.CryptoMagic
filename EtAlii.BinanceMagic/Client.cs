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

        public Client(Settings settings)
        {
            _settings = settings;
        }

        private void HandleFail(string message)
        {
            if (_settings.IsTest)
            {
                throw new InvalidOperationException(message);
            }

            ConsoleOutput.WriteNegative(message);
            Environment.Exit(-1);
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
                HandleFail(message);
            }

            ConsoleOutput.Write("Starting Binance client: Done");
            
        }

        public decimal GetPrice(string coin, CancellationToken cancellationToken)
        {
            var coinComparedToReference = $"{coin}{_settings.ReferenceCoin}"; 
            var result = _client.Spot.Market.GetPrice(coinComparedToReference, cancellationToken);
            if (result.Error != null)
            {
                var message = $"Failure fetching price for {coin}: {result.Error}";
                HandleFail(message);
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
                HandleFail(message);
            }

            var fees = result.Data.First();
            return (fees.MakerFee, fees.TakerFee);
        }
        
        public bool TryConvert(SellAction sellAction, BuyAction buyAction, CancellationToken cancellationToken, out Transaction transaction)
        {
            var sellCoin = $"{sellAction.Coin}{_settings.ReferenceCoin}";
            var sellOrder = _client.Spot.Order.PlaceTestOrder(sellCoin, OrderSide.Sell, OrderType.TakeProfitLimit,
                sellAction.Quantity, null, sellAction.TransactionId, sellAction.TargetPrice, TimeInForce.FillOrKill,
                sellAction.TargetPrice, null, OrderResponseType.Result, null, cancellationToken);

            if (sellOrder.Error != null)
            {
                var message = $"Failure placing sell order for {sellAction.Coin}: {sellOrder.Error}";
                HandleFail(message);
            }
            if (sellOrder.Data.Status != OrderStatus.Filled)
            {
                var message = $"Failure placing sell order for {sellAction.Coin}: {sellOrder.Data.Status}";
                HandleFail(message);
            }

            var buyCoin = $"{buyAction.Coin}{_settings.ReferenceCoin}";
            var buyOrder = _client.Spot.Order.PlaceTestOrder(buyCoin, OrderSide.Buy, OrderType.StopLossLimit,
                buyAction.Quantity, null, buyAction.TransactionId, buyAction.TargetPrice, TimeInForce.FillOrKill,
                buyAction.TargetPrice, null, OrderResponseType.Result, null, cancellationToken);

            if (buyOrder.Error != null)
            {
                var message = $"Failure placing buy order for {buyAction.Coin}: {buyOrder.Error}";
                HandleFail(message);
            }
            if (buyOrder.Data.Status != OrderStatus.Filled)
            {
                var message = $"Failure placing buy order for {buyAction.Coin}: {buyOrder.Data.Status}";
                HandleFail(message);
            }

            transaction = new Transaction
            {
                From = new CoinSnapshot
                {
                    Coin = sellOrder.Data.Symbol,
                    Price = sellOrder.Data.Price,
                    Quantity = sellOrder.Data.Quantity
                },
                To = new CoinSnapshot
                {
                    Coin = buyOrder.Data.Symbol,
                    Price = buyOrder.Data.Price,
                    Quantity = buyOrder.Data.Quantity
                },
                Moment = DateTime.Now,
                TotalProfit =  (buyOrder.Data.Price * buyOrder.Data.Quantity) - (sellOrder.Data.Price * sellOrder.Data.Quantity) 
            };
            return false;
        }

        public void Stop()
        {
            ConsoleOutput.Write("Stopping Binance client...");
            _client.Dispose();
            ConsoleOutput.Write("Stopping Binance client: Done");
        }
    }
}