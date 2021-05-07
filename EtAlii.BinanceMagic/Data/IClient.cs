namespace EtAlii.BinanceMagic
{
    using System;
    using System.Threading;
    using Binance.Net.Objects.Spot.MarketData;
    using EtAlii.BinanceMagic.Surfing;

    public interface IClient
    {
        void Start();
        void Stop();
        bool TryGetPrice(string coin, string referenceCoin, CancellationToken cancellationToken, out decimal price, out string error);

        bool TryGetTradeFees(string coin, string referenceCoin, CancellationToken cancellationToken, out decimal makerFee, out decimal takerFee, out string error);
        bool TryGetTrend(string coin, string referenceCoin, CancellationToken cancellationToken, out decimal trend, out string error);
        bool TryGetTrends(string[] coin, string referenceCoin, CancellationToken cancellationToken, out Trend[] trends, out string error);

        bool TryGetExchangeInfo(CancellationToken cancellationToken, out BinanceExchangeInfo exchangeInfo, out string error);
        
        bool TryConvert(SellAction sellAction, BuyAction buyAction, string referenceCoin, CancellationToken cancellationToken, Func<DateTime> getNow, out Transaction transaction, out string error);
        
        decimal GetMinimalQuantity(string coin, BinanceExchangeInfo exchangeInfo, string referenceCoin);
    }
}