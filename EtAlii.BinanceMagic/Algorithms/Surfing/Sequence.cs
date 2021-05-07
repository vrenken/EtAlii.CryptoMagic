namespace EtAlii.BinanceMagic.Surfing
{
    using System;
    using System.Threading;

    public class Sequence : ISequence
    {
        public IStatusProvider Status { get; }

        private readonly StateMachine _stateMachine;

        public Sequence(AlgorithmSettings settings)
        {
            _stateMachine = new StateMachine(settings);
        }
        
        public void Run(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}