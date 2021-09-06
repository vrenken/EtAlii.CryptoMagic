namespace EtAlii.CryptoMagic.Service
{
    using System;
    using System.Threading.Tasks;

    public class ExperimentalAlgorithmRunner : IAlgorithmRunner<ExperimentalTransaction, ExperimentalTrading>
    {
        public event Action<IAlgorithmRunner<ExperimentalTransaction, ExperimentalTrading>> Changed;
        public string Log => string.Empty;
        public IAlgorithmContext<ExperimentalTransaction, ExperimentalTrading> Context { get; }

        public ExperimentalAlgorithmRunner(ExperimentalTrading trading)
        {
            Context = new AlgorithmContext<ExperimentalTransaction, ExperimentalTrading>(trading);
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