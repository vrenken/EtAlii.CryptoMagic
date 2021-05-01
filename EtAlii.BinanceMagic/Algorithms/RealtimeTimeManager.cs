namespace EtAlii.BinanceMagic
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class RealtimeTimeManager : ITimeManager
    {
        public DateTime GetNow() => DateTime.Now;

        public void Wait(TimeSpan timeSpan, CancellationToken cancellationToken)
        {
            Task
                .Delay(timeSpan, cancellationToken)
                .Wait(cancellationToken);
        }
    }
}