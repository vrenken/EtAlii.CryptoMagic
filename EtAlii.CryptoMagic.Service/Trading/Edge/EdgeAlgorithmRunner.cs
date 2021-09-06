namespace EtAlii.CryptoMagic.Service
{
    using System;
    using System.Threading.Tasks;

    public class EdgeAlgorithmRunner : IAlgorithmRunner<EdgeTransaction, EdgeTrading>
    {
        public event Action<IAlgorithmRunner<EdgeTransaction, EdgeTrading>> Changed;
        public string Log { get; } = string.Empty;
        public IAlgorithmContext<EdgeTransaction, EdgeTrading> Context { get; }

        public EdgeAlgorithmRunner(EdgeTrading trading)
        {
            Context = new AlgorithmContext<EdgeTransaction, EdgeTrading>(trading);
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