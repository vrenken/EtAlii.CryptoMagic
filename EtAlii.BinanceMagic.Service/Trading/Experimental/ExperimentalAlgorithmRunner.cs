namespace EtAlii.BinanceMagic.Service
{
    using System.Threading;
    using System.Threading.Tasks;

    public class ExperimentalAlgorithmRunner : IAlgorithmRunner
    {
        public TradingBase Trading => _trading;
        private readonly ExperimentalTrading _trading;

        public ExperimentalAlgorithmRunner(ExperimentalTrading trading)
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