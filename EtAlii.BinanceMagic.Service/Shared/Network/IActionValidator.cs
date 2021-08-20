namespace EtAlii.BinanceMagic.Service
{
    using System.Threading;
    using System.Threading.Tasks;
    using Binance.Net;
    using Binance.Net.Objects.Spot.MarketData;

    public interface IActionValidator
    {
        Task<(bool success, TAction action, string error)> TryValidate<TAction>(BinanceClient client,
            TAction action,
            string type,
            string referenceCoin,
            BinanceExchangeInfo exchangeInfo,
            CancellationToken cancellationToken)
            where TAction : TradeAction;
    }
}