namespace EtAlii.BinanceMagic
{
    using System;
    using System.Threading;

    public class BackTestTimeManager : ITimeManager
    {
        private readonly BackTestClient _client;

        public BackTestTimeManager(BackTestClient client)
        {
            _client = client;
        }

        public DateTime GetNow() => _client.Moment;

        public void Wait(TimeSpan timeSpan, CancellationToken cancellationToken)
        {
            _client.Moment += _client.Interval;
            if (_client.Moment > _client.LastRecordedHistory)
            {
                
            }
        }
    }
}