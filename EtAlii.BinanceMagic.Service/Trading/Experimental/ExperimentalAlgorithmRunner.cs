namespace EtAlii.BinanceMagic.Service
{
    public class ExperimentalAlgorithmRunner : IAlgorithmRunner<ExperimentalTradeDetailsSnapshot, ExperimentalTrading>
    {
        public event System.Action Changed;
        public string Log => string.Empty;
        public IAlgorithmContext<ExperimentalTradeDetailsSnapshot, ExperimentalTrading> Context { get; }

        public ExperimentalAlgorithmRunner(ExperimentalTrading trading)
        {
            Context = new AlgorithmContext<ExperimentalTradeDetailsSnapshot, ExperimentalTrading>
            {
                Trading = trading,
            };
        }
        public void Start()
        {
            
        }

        public void Stop()
        {
        }
    }
}