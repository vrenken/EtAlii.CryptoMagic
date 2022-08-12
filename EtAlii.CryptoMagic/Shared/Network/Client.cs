namespace EtAlii.CryptoMagic
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Binance.Net.Clients;
    using Binance.Net.Enums;
    using Binance.Net.Objects;
    using Binance.Net.Objects.Models.Spot;
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

        public async Task Start(string apiKey, string secretKey)
        {
            _logger.Information("Starting client");

            if (_clientApiKey != apiKey || _clientSecretKey != secretKey)
            {
                var options = new BinanceClientOptions
                {
                    SpotApiOptions =
                    {
                        RateLimitingBehaviour = RateLimitingBehaviour.Wait,
                        TradeRulesBehaviour = TradeRulesBehaviour.AutoComply,
                    },
                    ApiCredentials = new ApiCredentials(apiKey, secretKey),
                };
                _client?.Dispose();
                
                _client = new BinanceClient(options);
                
                _clientApiKey = apiKey;
                _clientSecretKey = secretKey;
                
                var startResult = await _client.SpotApi.Account.StartUserStreamAsync();

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
            var response = _client.SpotApi.ExchangeData.GetExchangeInfoAsync();
            response.Wait();
            return response.Result.Data.Symbols
                .Where(s => s.QuoteAsset == referenceSymbol)
                .Select(s => new SymbolDefinition { Name = s.BaseAsset, Base = s.QuoteAsset })
                .ToArray();
        }

        public async Task<decimal> GetBalance(string symbol)
        {
            var response = await _client.SpotApi.Account.GetUserAssetsAsync();
            return response.Data.SingleOrDefault(c => c.Asset == symbol)?.Available ?? 0;
            //return response.Data.SingleOrDefault(c => c.Coin == symbol)?.Free ?? 0;            
        }

        public void Stop()
        {
            _logger.Information("Stopping client");
            _logger.Information("Stopping client: Done");
        }

        public async Task<(bool success, decimal price, string error)> TryGetPrice(string symbol, string referenceSymbol, CancellationToken cancellationToken)
        {
            var symbolComparedToReference = $"{symbol}{referenceSymbol}"; 
            var result = await _client.SpotApi.ExchangeData.GetPriceAsync(symbolComparedToReference, cancellationToken);
            if (result.Error != null)
            {
                return (false, decimal.Zero, $"Failure fetching price for {symbol}: {result.Error}");
            }

            return (true, result.Data.Price, null);
        }
        
        public async Task<(bool success, decimal makerFee, decimal takerFee, string error)> TryGetTradeFees(string symbol, string referenceSymbol, CancellationToken cancellationToken)
        {
            var coinComparedToReference = $"{symbol}{referenceSymbol}"; 
            var result = await _client.SpotApi.ExchangeData.GetTradeFeeAsync(coinComparedToReference, null, cancellationToken);
            if (result.Error != null)
            {
                return (false, 0m, 0m, $"Failure fetching trade fees for {symbol}: {result.Error}");
            }

            var fees = result.Data.First();
            return (true, fees.MakerFee, fees.TakerFee, null);
        }
        
        public async Task<(bool success, BinanceExchangeInfo exchangeInfo, string error)> TryGetExchangeInfo(CancellationToken cancellationToken)
        {
            var exchangeResult = await _client.SpotApi.ExchangeData.GetExchangeInfoAsync(cancellationToken);
            if (!exchangeResult.Success)
            {
                return (false, null, $"Failed to fetch exchange info: {exchangeResult.Error}");
            }
            return (true, exchangeResult.Data, null);
        }
        
        public decimal GetMinimalQuantity(string coin, BinanceExchangeInfo exchangeInfo, string referenceCoin)
        {
            var symbol = exchangeInfo.Symbols.Single(s => s.BaseAsset == coin && s.QuoteAsset == referenceCoin);
            return symbol.MinNotionalFilter!.MinNotional;
        }

        public async Task<(bool success, string error)> TryHasSufficientQuota(string symbol, decimal minimumValue)
        {
            var result = await _client.SpotApi.Account.GetUserAssetsAsync();
            if (!result.Success)
            {
                return (false, $"Failure checking for quota of {symbol}: {result.Error}");
            }

            //var coin = result.Data.SingleOrDefault(c => c.Coin == symbol);
            var coin = result.Data.SingleOrDefault(c => c.Asset == symbol);
            if (coin == null)
            {
                return (false, $"Failure checking for quota of {symbol}: Coin not found");
            }

            return (coin.Available > minimumValue, null);
            // return (coin.Free > minimumValue, null);            
        }
    }
}