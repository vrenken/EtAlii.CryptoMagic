namespace EtAlii.BinanceMagic.Service
{
    public class SurfingAlgorithmRunner : IAlgorithmRunner<SurfingTradeDetailsSnapshot, SurfingTrading>
    {
        public event System.Action Changed;
        
        public string Log { get; } = string.Empty;
        public IAlgorithmContext<SurfingTradeDetailsSnapshot, SurfingTrading> Context { get; }

        public SurfingAlgorithmRunner(SurfingTrading trading)
        {
            Context = new AlgorithmContext<SurfingTradeDetailsSnapshot, SurfingTrading>
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