﻿namespace EtAlii.BinanceMagic
{
    using System.Linq;
    using System.Threading;
    using Binance.Net;
    using Binance.Net.Enums;
    using Binance.Net.Objects.Spot;
    using Binance.Net.Objects.Spot.MarketData;
    using CryptoExchange.Net.Authentication;
    using CryptoExchange.Net.Objects;

    public partial class Client : IClient
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

        public void Stop()
        {
            _output.WriteLine("Stopping client...");
            _client.Dispose();
            _output.WriteLine("Stopping client: Done");
        }

        public bool TryGetPrice(string coin, string referenceCoin, CancellationToken cancellationToken, out decimal price, out string error)
        {
            var coinComparedToReference = $"{coin}{referenceCoin}"; 
            var result = _client.Spot.Market.GetPrice(coinComparedToReference, cancellationToken);
            if (result.Error != null)
            {
                error = $"Failure fetching price for {coin}: {result.Error}";
                price = decimal.Zero;
                return false;
            }

            price = result.Data.Price;
            error = null;
            return true;
        }
        
        public bool TryGetTradeFees(string coin, string referenceCoin, CancellationToken cancellationToken, out decimal makerFee, out decimal takerFee, out string error)
        {
            var coinComparedToReference = $"{coin}{referenceCoin}"; 
            var result = _client.Spot.Market.GetTradeFee(coinComparedToReference, null, cancellationToken);
            if (result.Error != null)
            {
                error = $"Failure fetching trade fees for {coin}: {result.Error}";
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