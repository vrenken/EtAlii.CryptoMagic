namespace EtAlii.CryptoMagic
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Binance.Net.Objects.Spot.MarketData;
    using EtAlii.CryptoMagic.Surfing;

    public interface IClient
    {
        Task Start(string apiKey, string secretKey);
        void Stop();

        Task<(bool success, string error)> TryHasSufficientQuota(string symbol, decimal minimumValue);
        
        Task<(bool success, decimal price, string error)> TryGetPrice(string symbol, string referenceSymbol, CancellationToken cancellationToken);

        Task<(bool success, decimal makerFee, decimal takerFee, string error)> TryGetTradeFees(string symbol, string referenceSymbol, CancellationToken cancellationToken);

        Task<(bool success, decimal trend, string error)> TryGetTrend(string symbol, string referenceSymbol, int period, CancellationToken cancellationToken);
        
        Task<(bool success, Trend[] trends, string error)> TryGetTrends(string[] symbols, string referenceSymbol, int period, CancellationToken cancellationToken);

        Task<(bool success, BinanceExchangeInfo exchangeInfo, string error)> TryGetExchangeInfo(CancellationToken cancellationToken);
        
        
        decimal GetMinimalQuantity(string coin, BinanceExchangeInfo exchangeInfo, string referenceCoin);

        Task<(bool success, TradeTransaction transaction, string error)> TryConvert(SellAction sellAction, BuyAction buyAction, string referenceSymbol, CancellationToken cancellationToken, Func<DateTime> getNow);
        Task<(bool success, Symbol symbolsSold, string error)> TrySell(SellAction sellAction, string referenceSymbol, CancellationToken cancellationToken, Func<DateTime> getNow);

        Task<(bool success, TradeTransaction transaction, string error)> TryBuyTransaction(BuyAction buyAction, string referenceSymbol, CancellationToken cancellationToken, Func<DateTime> getNow);
        Task<(bool success, Symbol symbolsBought, string error)> TryBuySymbol(BuyAction buyAction, string referenceSymbol, CancellationToken cancellationToken, Func<DateTime> getNow);

        SymbolDefinition[] GetSymbols(string referenceSymbol);
        Task<decimal> GetBalance(string symbol);
    }
}