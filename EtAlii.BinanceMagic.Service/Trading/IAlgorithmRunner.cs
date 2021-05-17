namespace EtAlii.BinanceMagic.Service
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