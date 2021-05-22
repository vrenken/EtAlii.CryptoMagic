namespace EtAlii.BinanceMagic.Service
{
    public class SimpleAlgorithmRunner : IAlgorithmRunner<SimpleTrading>
    {
        public event System.Action Changed;
        
        public string Log { get; } = string.Empty;
        public SimpleTrading Trading { get; }

        public SimpleAlgorithmRunner(SimpleTrading trading)
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