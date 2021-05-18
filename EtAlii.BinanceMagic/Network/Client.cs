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
    using Serilog;

    public partial class Client : IClient
    {
        private readonly ILogger _logger = Log.ForContext<Client>();
        private BinanceClient _client;
        private readonly IActionValidator _validator;

        public bool PlaceTestOrders { get; init; }

        public Client(IActionValidator actionValidator)
        {
            _validator = actionValidator;
        }

        public void Start(string apiKey, string secretKey)
        {
            _logger.Information("Starting client");
            
            var options = new BinanceClientOptions
            {
                RateLimitingBehaviour = RateLimitingBehaviour.Wait,
                ApiCredentials = new ApiCredentials(apiKey, secretKey),
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
                throw new InvalidOperationException(message);
            }
            
            _logger.Information("Starting client: Done");
        }

        public void Stop()
        {
            _logger.Information("Stopping client");
            _client.Dispose();
            _logger.Information("Stopping client: Done");
        }

        public bool TryGetPrice(string symbol, string referenceSymbol, CancellationToken cancellationToken, out decimal price, out string error)
        {
            var symbolComparedToReference = $"{symbol}{referenceSymbol}"; 
            var result = _client.Spot.Market.GetPrice(symbolComparedToReference, cancellationToken);
            if (result.Error != null)
            {
                error = $"Failure fetching price for {symbol}: {result.Error}";
                price = decimal.Zero;
                return false;
            }

            price = result.Data.Price;
            error = null;
            return true;
        }
        
        public bool TryGetTradeFees(string symbol, string referenceSymbol, CancellationToken cancellationToken, out decimal makerFee, out decimal takerFee, out string error)
        {
            var coinComparedToReference = $"{symbol}{referenceSymbol}"; 
            var result = _client.Spot.Market.GetTradeFee(coinComparedToReference, null, cancellationToken);
            if (result.Error != null)
            {
                error = $"Failure fetching trade fees for {symbol}: {result.Error}";
                makerFee = 0m;
                takerFee = 0m;
                return false;
            }

            var fees = result.Data.First();
            makerFee = fees.MakerFee;
            takerFee = fees.TakerFee;
            error = null;
            return true;
        }
        
        public bool TryGetExchangeInfo(CancellationToken cancellationToken, out BinanceExchangeInfo exchangeInfo, out string error)
        {
            var exchangeResult = _client.Spot.System.GetExchangeInfo(cancellationToken);
            if (!exchangeResult.Success)
            {
                error = $"Failed to fetch exchange info: {exchangeResult.Error}";
                exchangeInfo = null;
                return false;
            }
            exchangeInfo = exchangeResult.Data;
            error = null;
            return true;
        }
        
        public decimal GetMinimalQuantity(string coin, BinanceExchangeInfo exchangeInfo, string referenceCoin)
        {
            var symbol = exchangeInfo.Symbols.Single(s => s.BaseAsset == coin && s.QuoteAsset == referenceCoin);
            return symbol.MinNotionalFilter!.MinNotional;
        }
    }
}