namespace EtAlii.BinanceMagic.Service
{
    using System.Threading;
    using System.Threading.Tasks;

    public class CircularAlgorithmRunner : IAlgorithmRunner
    {
        public TradingBase Trading => _trading;
        private readonly CircularTrading _trading;

        public CircularAlgorithmRunner(CircularTrading trading)
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