namespace EtAlii.BinanceMagic.Service
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
        private static BinanceClient _client;
        private static string _clientApiKey;
        private static string _clientSecretKey;
        
        private readonly IActionValidator _validator;

        public bool PlaceTestOrders { get; init; }

        public Client(IActionValidator actionValidator)
        {
            _validator = actionValidator;
        }

        public void Start(string apiKey, string secretKey)
        {
            _logger.Information("Starting client");

            if (_clientApiKey != apiKey || _clientSecretKey != secretKey)
            {
                var options = new BinanceClientOptions
                {
                    RateLimitingBehaviour = RateLimitingBehaviour.Wait,
                    ApiCredentials = new ApiCredentials(apiKey, secretKey),
                    TradeRulesBehaviour = TradeRulesBehaviour.AutoComply,
                };
                _client?.Dispose();
                
                _client = new BinanceClient(options);
                
                _clientApiKey = apiKey;
                _clientSecretKey = secretKey;
                
                var startResult = _client.Spot.UserStream.StartUserStream();

                if (!startResult.Success)
                {
                    var message = $"Failed to start user stream: {startResult.Error}";
                    throw new InvalidOperationException(message);
                }
            }
            
            _logger.Information("Starting client: Done");
        }

        public SymbolDefinition[] GetSymbols(string referenceSymbol)
        {
            var response = _client.Spot.System.GetExchangeInfo();
            return response.Data.Symbols
                .Where(s => s.QuoteAsset == referenceSymbol)
                .Select(s => new SymbolDefinition { Name = s.BaseAsset, Base = s.QuoteAsset })
                .ToArray();
        }

        public void Stop()
        {
            _logger.Information("Stopping client");
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

        public bool TryHasSufficientQuota(string symbol, decimal minimumValue, out string error)
        {
            var result = _client.General.GetUserCoins();
            if (!result.Success)
            {
                error = $"Failure checking for quota of {symbol}: {result.Error}";
                return false;
            }

            var coin = result.Data.SingleOrDefault(c => c.Coin == symbol);
            if (coin == null)
            {
                error = $"Failure checking for quota of {symbol}: Coin not found";
                return false;
            }

            error = null;
            return coin.Free > minimumValue;
        }
    }
}