namespace EtAlii.BinanceMagic.Service
{
    using System;

    public class OneOffAlgorithmRunner : IAlgorithmRunner<OneOffTransaction, OneOffTrading>
    {
        public event Action Changed;
        public string Log { get; } = string.Empty;
        public IAlgorithmContext<OneOffTransaction, OneOffTrading> Context { get; }

        public OneOffAlgorithmRunner(OneOffTrading trading)
        {
            Context = new AlgorithmContext<OneOffTransaction, OneOffTrading>(trading);
        }
        public void Start()
        {
            
        }

        public void Stop()
        {
        }
    }
}