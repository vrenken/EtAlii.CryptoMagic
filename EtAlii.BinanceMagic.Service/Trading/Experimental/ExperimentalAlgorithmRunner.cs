namespace EtAlii.BinanceMagic.Service
{
    public class ExperimentalAlgorithmRunner : IAlgorithmRunner<ExperimentalTransaction, ExperimentalTrading>
    {
        public event System.Action Changed;
        public string Log => string.Empty;
        public IAlgorithmContext<ExperimentalTransaction, ExperimentalTrading> Context { get; }

        public ExperimentalAlgorithmRunner(ExperimentalTrading trading)
        {
            Context = new AlgorithmContext<ExperimentalTransaction, ExperimentalTrading>(trading);
        }
        public void Start()
        {
            
        }

        public void Stop()
        {
        }
    }
}