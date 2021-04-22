namespace EtAlii.BinanceMagic
{
    using System;
    using System.Linq;
    using System.Threading;
    using Binance.Net;
    using Binance.Net.Enums;
    using Binance.Net.Objects.Spot;
    using Binance.Net.Objects.Spot.MarketData;
    using CryptoExchange.Net.Authentication;
    using CryptoExchange.Net.Objects;

    public class Client
    {
        private BinanceClient _client;
        private BinanceSocketClient _socketClient;
        public void Start()
        {
            ConsoleOutput.Write("Starting Binance client...");
            
            var options = new BinanceClientOptions
            {
                RateLimitingBehaviour = RateLimitingBehaviour.Wait,
                ApiCredentials = new ApiCredentials(MagicAlgorithm.ApiKey, MagicAlgorithm.SecretKey),
            };
            _client = new BinanceClient(options);

            var socketOptions = new BinanceSocketClientOptions
            {
                ApiCredentials = new ApiCredentials(MagicAlgorithm.ApiKey, MagicAlgorithm.SecretKey),
            };
            _socketClient = new BinanceSocketClient(socketOptions);
            var result = _socketClient.Spot.SubscribeToSymbolMiniTickerUpdates(new[] {""}, b => b.);

            var startResult = _client.Spot.UserStream.StartUserStream();

            if (!startResult.Success)
            {
                var message = $"Failed to start user stream: {startResult.Error}";
                ConsoleOutput.WriteNegative(message);
                Environment.Exit(-1);
            }

            ConsoleOutput.Write("Starting Binance client: Done");
            
        }

        public decimal GetPrice(string coin, CancellationToken cancellationToken)
        {
            var coinComparedToReference = $"{coin}{MagicAlgorithm.ReferenceCoin}"; 
            var result = _client.Spot.Market.GetPrice(coinComparedToReference, cancellationToken);
            if (result.Error != null)
            {
                var message = $"Failure fetching price for {coin}: {result.Error}";
                ConsoleOutput.WriteNegative(message);
                Environment.Exit(-1);
            }

            return result.Data.Price;
        }

        public (decimal MakerFee, decimal TakerFee) GetTradeFees(string coin, CancellationToken cancellationToken)
        {
            var coinComparedToReference = $"{coin}{MagicAlgorithm.ReferenceCoin}"; 
            var result = _client.Spot.Market.GetTradeFee(coinComparedToReference, null, cancellationToken);
            if (result.Error != null)
            {
                var message = $"Failure fetching trade fees for {coin}: {result.Error}";
                ConsoleOutput.WriteNegative(message);
                Environment.Exit(-1);
            }

            var fees = result.Data.First();
            return (MakerFee: fees.MakerFee, TakerFee: fees.TakerFee);
        }
        
        public bool TryConvert(Target target, Situation situation)
        {
            var order = _client.Spot.Order.PlaceOrder()
            //_client.Spot.Market.GetTradeFee().Order.PlaceOrder(target.TargetCoin, OrderSide.Buy,  )

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