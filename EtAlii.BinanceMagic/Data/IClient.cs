namespace EtAlii.BinanceMagic
{
    using System.Threading;
    using Binance.Net.Objects.Spot.MarketData;

    public interface IClient
    {
        void Start();
        void Stop();
        decimal GetPrice(string coin, string referenceCoin, CancellationToken cancellationToken);

        bool TryGetTradeFees(string coin, string referenceCoin, StatusInfo statusInfo, CancellationToken cancellationToken, out decimal makerFee, out decimal takerFee);

        bool TryGetExchangeInfo(StatusInfo statusInfo, CancellationToken cancellationToken, out BinanceExchangeInfo exchangeInfo);
        
        bool TryConvert(SellAction sellAction, BuyAction buyAction, string referenceCoin, StatusInfo statusInfo, CancellationToken cancellationToken, out Transaction transaction);
    }
}