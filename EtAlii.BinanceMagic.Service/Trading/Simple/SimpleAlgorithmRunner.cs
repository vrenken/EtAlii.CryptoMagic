namespace EtAlii.BinanceMagic.Service
{
    public class SimpleAlgorithmRunner : IAlgorithmRunner<SimpleTradeDetailsSnapshot, SimpleTrading>
    {
        public event System.Action Changed;
        
        public string Log { get; } = string.Empty;
        public IAlgorithmContext<SimpleTradeDetailsSnapshot, SimpleTrading> Context { get; }

        public SimpleAlgorithmRunner(SimpleTrading trading)
        {
            Context = new AlgorithmContext<SimpleTradeDetailsSnapshot, SimpleTrading>
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