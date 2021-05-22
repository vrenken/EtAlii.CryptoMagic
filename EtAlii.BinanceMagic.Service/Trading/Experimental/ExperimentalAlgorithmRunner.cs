namespace EtAlii.BinanceMagic.Service
{
    public class ExperimentalAlgorithmRunner : IAlgorithmRunner<ExperimentalTrading>
    {
        public event System.Action Changed;
        public string Log => string.Empty;
        public ExperimentalTrading Trading { get; }

        public ExperimentalAlgorithmRunner(ExperimentalTrading trading)
        {
            Trading = trading;
        }
        public void Start()
        {
            
        }

        public void Stop()
        {
        }
    }
}