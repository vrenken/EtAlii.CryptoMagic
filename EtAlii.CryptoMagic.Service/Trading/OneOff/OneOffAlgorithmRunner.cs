namespace EtAlii.CryptoMagic.Service
{
    using System;
    using System.Threading;

    public partial class OneOffAlgorithmRunner : IAlgorithmRunner<OneOffTransaction, OneOffTrading>
    {
        private Client _client;
        private readonly OneOffTrading _trading;
        public event Action<IAlgorithmRunner<OneOffTransaction, OneOffTrading>> Changed;
        public string Log { get; } = string.Empty;

        private Timer _timer;
        public IAlgorithmContext<OneOffTransaction, OneOffTrading> Context { get; }

        public OneOffAlgorithmRunner(OneOffTrading trading)
        {
            _trading = trading;
            Context = new AlgorithmContext<OneOffTransaction, OneOffTrading>(trading);
        }
    }
}