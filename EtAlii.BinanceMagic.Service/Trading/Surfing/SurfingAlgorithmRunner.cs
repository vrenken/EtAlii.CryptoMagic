namespace EtAlii.BinanceMagic.Service
{
    using System.Threading;
    using System.Threading.Tasks;

    public class SurfingAlgorithmRunner : IAlgorithmRunner
    {
        public TradingBase Trading => _trading;
        private readonly SurfingTrading _trading;

        public SurfingAlgorithmRunner(SurfingTrading trading)
        {
            _trading = trading;
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