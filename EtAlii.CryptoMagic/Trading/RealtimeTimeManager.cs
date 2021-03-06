namespace EtAlii.CryptoMagic
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class RealtimeTimeManager : ITimeManager
    {
        public DateTime GetNow() => DateTime.Now;

        public bool ShouldStop() => false;

        public void Wait(TimeSpan timeSpan, CancellationToken cancellationToken)
        {
            Task
                .Delay(timeSpan, cancellationToken)
                .Wait(cancellationToken);
        }
    }
}