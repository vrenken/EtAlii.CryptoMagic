namespace EtAlii.BinanceMagic.Surfing
{
    using System;
    using System.Threading;

    public class SurfingSequence : ISequence
    {
        public IStatusProvider Status { get; }
        
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