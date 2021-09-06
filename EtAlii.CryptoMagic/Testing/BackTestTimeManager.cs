namespace EtAlii.CryptoMagic
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class BackTestTimeManager : ITimeManager
    {
        public BackTestClient Client { get; init; }
        
        public DateTime GetNow() => Client.Moment;

        public bool ShouldStop()
        {
            return Client.Moment > Client.LastRecordedHistory;
        }
        public void Wait(TimeSpan timeSpan, CancellationToken cancellationToken)
        {
            Client.Moment += Client.Interval;

            Task.Delay(TimeSpan.FromMilliseconds(2), cancellationToken).Wait(cancellationToken);
        }
    }
}