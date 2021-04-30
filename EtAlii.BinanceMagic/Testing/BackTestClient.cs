namespace EtAlii.BinanceMagic
{
    using System.Threading;
    using Binance.Net.Objects.Spot.MarketData;

    public class BackTestClient : IClient
    {
        public void Start() { }
        public void Stop() { }

        public BackTestClient(string[] coins)
        {
            
        }
        public bool TryGetPrice(string coin, string referenceCoin, TradeDetails details, CancellationToken cancellationToken, out decimal price)
        {
            
        }

        
        public bool TryConvert(SellAction sellAction, BuyAction buyAction, string referenceCoin, TradeDetails details,
            CancellationToken cancellationToken, out Transaction transaction)
        {
            
        }

        public bool TryGetTradeFees(string coin, string referenceCoin, TradeDetails details,
            CancellationToken cancellationToken, out decimal makerFee, out decimal takerFee)
        {
            makerFee = 0.1m;
            takerFee = 0.1m;
            return true;
        }

        public bool TryGetTrend(string coin, string referenceCoin, TradeDetails details,
            CancellationToken cancellationToken, out decimal trend)
        {
            
        }

        public bool TryGetExchangeInfo(TradeDetails details, CancellationToken cancellationToken,
            out BinanceExchangeInfo exchangeInfo)
        {
            exchangeInfo = null;
            return true;
        }
    }
}