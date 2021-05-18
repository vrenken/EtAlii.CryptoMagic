namespace EtAlii.BinanceMagic.Service
{
    public class SurfingAlgorithmRunner : IAlgorithmRunner
    {
        public string Log { get; } = string.Empty;
        public TradingBase Trading => _trading;
        private readonly SurfingTrading _trading;

        public SurfingAlgorithmRunner(SurfingTrading trading)
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