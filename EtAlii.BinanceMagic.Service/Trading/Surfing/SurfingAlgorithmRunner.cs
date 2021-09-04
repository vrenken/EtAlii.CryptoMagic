namespace EtAlii.BinanceMagic.Service.Surfing
{
    public class SurfingAlgorithmRunner : IAlgorithmRunner<SurfingTransaction, SurfingTrading>
    {
        public event System.Action Changed;
        
        public string Log { get; } = string.Empty;
        public IAlgorithmContext<SurfingTransaction, SurfingTrading> Context { get; }

        public SurfingAlgorithmRunner(SurfingTrading trading)
        {
            Context = new AlgorithmContext<SurfingTransaction, SurfingTrading>(trading);
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