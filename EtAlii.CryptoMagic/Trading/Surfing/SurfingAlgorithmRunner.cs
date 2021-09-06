namespace EtAlii.CryptoMagic.Surfing
{
    using System;
    using System.Threading.Tasks;

    public class SurfingAlgorithmRunner : IAlgorithmRunner<SurfingTransaction, SurfingTrading>
    {
        public event Action<IAlgorithmRunner<SurfingTransaction, SurfingTrading>> Changed;
        
        public string Log { get; } = string.Empty;
        public IAlgorithmContext<SurfingTransaction, SurfingTrading> Context { get; }

        public SurfingAlgorithmRunner(SurfingTrading trading)
        {
            Context = new AlgorithmContext<SurfingTransaction, SurfingTrading>(trading);
        }
        public Task Start()
        {
            Changed?.Invoke(this);
            return Task.CompletedTask;
        }

        public Task Stop()
        {
            Changed?.Invoke(this);
            return Task.CompletedTask;
        }
    }
}