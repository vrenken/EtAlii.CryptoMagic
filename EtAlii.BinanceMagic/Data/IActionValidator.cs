namespace EtAlii.BinanceMagic
{
    using System.Threading;
    using Binance.Net;
    using Binance.Net.Objects.Spot.MarketData;

    public interface IActionValidator
    {
        bool TryValidate<TAction>(BinanceClient client,
            TAction action,
            string type,
            string referenceCoin,
            BinanceExchangeInfo exchangeInfo,
            TradeDetails details,
            CancellationToken cancellationToken,
            out TAction outAction)
            where TAction : Action;
    }
}