namespace EtAlii.BinanceMagic.Service
{
    public class SimpleAlgorithmRunner : IAlgorithmRunner
    {
        public string Log { get; } = string.Empty;
        public TradingBase Trading => _trading;
        private readonly SimpleTrading _trading;

        public SimpleAlgorithmRunner(SimpleTrading trading)
        {
            _trading = trading;
        }
        public void Start()
        {
            
        }

        public void Stop()
        {
        }
    }
}