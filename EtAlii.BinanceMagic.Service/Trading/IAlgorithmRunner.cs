namespace EtAlii.BinanceMagic.Service.Trading
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAlgorithmRunner
    {
        TradingBase Trading { get; }
        void Run();

        Task StopAsync(CancellationToken cancellationToken);
    }
}