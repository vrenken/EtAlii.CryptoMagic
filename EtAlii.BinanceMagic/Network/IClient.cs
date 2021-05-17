namespace EtAlii.BinanceMagic
{
    using System;
    using System.Threading;
    using Binance.Net.Objects.Spot.MarketData;
    using EtAlii.BinanceMagic.Surfing;

    public interface IClient
    {
        void Start(string apiKey, string secretKey);
        void Stop();
        bool TryGetPrice(string coin, string referenceCoin, CancellationToken cancellationToken, out decimal price, out string error);

        bool TryGetTradeFees(string coin, string referenceCoin, CancellationToken cancellationToken, out decimal makerFee, out decimal takerFee, out string error);
        bool TryGetTrend(string coin, string referenceCoin, CancellationToken cancellationToken, out decimal trend, out string error);
        bool TryGetTrends(string[] coin, string referenceCoin, int period, CancellationToken cancellationToken, out Trend[] trends, out string error);

        bool TryGetExchangeInfo(CancellationToken cancellationToken, out BinanceExchangeInfo exchangeInfo, out string error);
        
        
        decimal GetMinimalQuantity(string coin, BinanceExchangeInfo exchangeInfo, string referenceCoin);

        bool TryConvert(SellAction sellAction, BuyAction buyAction, string referenceCoin, CancellationToken cancellationToken, Func<DateTime> getNow, out Transaction transaction, out string error);
        bool TrySell(SellAction sellAction, string referenceCoin, CancellationToken cancellationToken, Func<DateTime> getNow, out Coin coinsSold, out string error);

        bool TryBuy(BuyAction buyAction, string referenceCoin, CancellationToken cancellationToken, Func<DateTime> getNow, out Coin coinsBought, out string error);
    }
}