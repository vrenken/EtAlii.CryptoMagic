namespace EtAlii.BinanceMagic.Service
{
    public class SurfingAlgorithmRunner : IAlgorithmRunner<SurfingTrading>
    {
        public event System.Action Changed;
        
        public string Log { get; } = string.Empty;
        public SurfingTrading Trading { get; }

        public SurfingAlgorithmRunner(SurfingTrading trading)
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