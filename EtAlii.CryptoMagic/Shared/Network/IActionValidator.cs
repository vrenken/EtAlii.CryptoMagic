namespace EtAlii.CryptoMagic
{
    using System.Threading;
    using System.Threading.Tasks;
    using Binance.Net.Clients;
    using Binance.Net.Objects.Models.Spot;

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