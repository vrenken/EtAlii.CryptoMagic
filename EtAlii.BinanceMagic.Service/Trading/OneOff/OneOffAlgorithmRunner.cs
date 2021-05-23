namespace EtAlii.BinanceMagic.Service
{
    using System;

    public class OneOffAlgorithmRunner : IAlgorithmRunner<Object, OneOffTrading>
    {
        public event Action Changed;
        public string Log { get; } = string.Empty;
        public IAlgorithmContext<Object, OneOffTrading> Context { get; }

        public OneOffAlgorithmRunner(OneOffTrading trading)
        {
            Context = new AlgorithmContext<Object, OneOffTrading>
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