namespace EtAlii.BinanceMagic.Service
{
    public class SimpleAlgorithmRunner : IAlgorithmRunner<SimpleTransaction, SimpleTrading>
    {
        public event System.Action Changed;
        
        public string Log { get; } = string.Empty;
        public IAlgorithmContext<SimpleTransaction, SimpleTrading> Context { get; }

        public SimpleAlgorithmRunner(SimpleTrading trading)
        {
            Context = new AlgorithmContext<SimpleTransaction, SimpleTrading>(trading);
        }
        public void Start()
        {
            Changed?.Invoke();
        }

        public void Stop()
        {
            Changed?.Invoke();
        }
    }
}