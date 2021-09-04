namespace EtAlii.BinanceMagic.Service
{
    using System;

    public class EdgeAlgorithmRunner : IAlgorithmRunner<EdgeTransaction, EdgeTrading>
    {
        public event Action Changed;
        public string Log { get; } = string.Empty;
        public IAlgorithmContext<EdgeTransaction, EdgeTrading> Context { get; }

        public EdgeAlgorithmRunner(EdgeTrading trading)
        {
            Context = new AlgorithmContext<EdgeTransaction, EdgeTrading>(trading);
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