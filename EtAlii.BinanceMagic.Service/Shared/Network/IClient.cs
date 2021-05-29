namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Threading;
    using Binance.Net.Objects.Spot.MarketData;
    using EtAlii.BinanceMagic.Service.Surfing;

    public interface IClient
    {
        void Start(string apiKey, string secretKey);
        void Stop();

        bool TryHasSufficientQuota(string symbol, decimal minimumValue, out string error);
        
        bool TryGetPrice(string symbol, string referenceSymbol, CancellationToken cancellationToken, out decimal price, out string error);

        bool TryGetTradeFees(string symbol, string referenceSymbol, CancellationToken cancellationToken, out decimal makerFee, out decimal takerFee, out string error);
        bool TryGetTrend(string symbol, string referenceSymbol, int period, CancellationToken cancellationToken, out decimal trend, out string error);
        bool TryGetTrends(string[] symbols, string referenceSymbol, int period, CancellationToken cancellationToken, out Trend[] trends, out string error);

        bool TryGetExchangeInfo(CancellationToken cancellationToken, out BinanceExchangeInfo exchangeInfo, out string error);
        
        
        decimal GetMinimalQuantity(string coin, BinanceExchangeInfo exchangeInfo, string referenceCoin);

        bool TryConvert(SellAction sellAction, BuyAction buyAction, string referenceSymbol, CancellationToken cancellationToken, Func<DateTime> getNow, out TradeTransaction transaction, out string error);
        bool TrySell(SellAction sellAction, string referenceSymbol, CancellationToken cancellationToken, Func<DateTime> getNow, out Symbol symbolsSold, out string error);

        bool TryBuy(BuyAction buyAction, string referenceSymbol, CancellationToken cancellationToken, Func<DateTime> getNow, out TradeTransaction transaction, out string error);
        bool TryBuy(BuyAction buyAction, string referenceSymbol, CancellationToken cancellationToken, Func<DateTime> getNow, out Symbol symbolsBought, out string error);

        SymbolDefinition[] GetSymbols(string referenceSymbol);
    }
}