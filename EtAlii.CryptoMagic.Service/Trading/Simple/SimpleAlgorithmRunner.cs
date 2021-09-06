namespace EtAlii.CryptoMagic.Service
{
    using System;
    using System.Threading.Tasks;

    public class SimpleAlgorithmRunner : IAlgorithmRunner<SimpleTransaction, SimpleTrading>
    {
        public event Action<IAlgorithmRunner<SimpleTransaction, SimpleTrading>> Changed;
        
        public string Log { get; } = string.Empty;
        public IAlgorithmContext<SimpleTransaction, SimpleTrading> Context { get; }

        public SimpleAlgorithmRunner(SimpleTrading trading)
        {
            Context = new AlgorithmContext<SimpleTransaction, SimpleTrading>(trading);
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