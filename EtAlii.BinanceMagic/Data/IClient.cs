namespace EtAlii.BinanceMagic
{
    using System.Threading;
    using Binance.Net.Objects.Spot.MarketData;

    public interface IClient
    {
        void Start();
        void Stop();
        bool TryGetPrice(string coin, string referenceCoin, TradeDetails details, CancellationToken cancellationToken, out decimal price);

        bool TryGetTradeFees(string coin, string referenceCoin, TradeDetails details, CancellationToken cancellationToken, out decimal makerFee, out decimal takerFee);

        bool TryGetExchangeInfo(TradeDetails details, CancellationToken cancellationToken, out BinanceExchangeInfo exchangeInfo);
        
        bool TryConvert(SellAction sellAction, BuyAction buyAction, string referenceCoin, TradeDetails details, CancellationToken cancellationToken, out Transaction transaction);
    }
}