namespace EtAlii.BinanceMagic.Service
{
    public class ExperimentalAlgorithmRunner : IAlgorithmRunner
    {
        public string Log { get; } = string.Empty;
        public TradingBase Trading => _trading;
        private readonly ExperimentalTrading _trading;

        public ExperimentalAlgorithmRunner(ExperimentalTrading trading)
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