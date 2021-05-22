namespace EtAlii.BinanceMagic.Service
{
    public class OneOffAlgorithmRunner : IAlgorithmRunner<OneOffTrading>
    {
        public event System.Action Changed;
        public string Log { get; } = string.Empty;
        public OneOffTrading Trading { get; }

        public OneOffAlgorithmRunner(OneOffTrading trading)
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