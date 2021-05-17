namespace EtAlii.BinanceMagic.Service
{
    using System.Threading;
    using System.Threading.Tasks;

    public class OneOffAlgorithmRunner : IAlgorithmRunner
    {
        public TradingBase Trading => _trading;
        private readonly OneOffTrading _trading;

        public OneOffAlgorithmRunner(OneOffTrading trading)
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