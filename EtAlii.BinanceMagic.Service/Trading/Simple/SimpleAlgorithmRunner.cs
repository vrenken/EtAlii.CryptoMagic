namespace EtAlii.BinanceMagic.Service.Trading.Simple
{
    using System.Threading;
    using System.Threading.Tasks;

    public class SimpleAlgorithmRunner : IAlgorithmRunner
    {
        public TradingBase Trading { get; }

        public SimpleAlgorithmRunner(TradingBase trading)
        {
            Trading = trading;
        }
        public void Run()
        {
            
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}