namespace EtAlii.BinanceMagic.Service
{
    public class OneOffAlgorithmRunner : IAlgorithmRunner
    {
        public event System.Action Changed;
        public string Log { get; } = string.Empty;
        public TradingBase Trading => _trading;
        private readonly OneOffTrading _trading;

        public OneOffAlgorithmRunner(OneOffTrading trading)
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