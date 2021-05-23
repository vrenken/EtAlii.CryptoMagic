namespace EtAlii.BinanceMagic.Service
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
            CancellationToken cancellationToken,
            out TAction outAction,
            out string error)
            where TAction : TradeAction;
    }
}