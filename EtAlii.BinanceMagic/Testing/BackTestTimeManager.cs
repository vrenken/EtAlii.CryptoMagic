namespace EtAlii.BinanceMagic
{
    using System;
    using System.Threading;

    public class BackTestTimeManager : ITimeManager
    {
        private readonly BackTestClient _client;
        private readonly IProgram _program;

        public BackTestTimeManager(BackTestClient client, IProgram program)
        {
            _client = client;
            _program = program;
        }

        public DateTime GetNow() => _client.Moment;

        public void Wait(TimeSpan timeSpan, CancellationToken cancellationToken)
        {
            _client.Moment += _client.Interval;

            //Task.Delay(TimeSpan.FromMilliseconds(10), cancellationToken).Wait(cancellationToken);
            if (_client.Moment > _client.LastRecordedHistory)
            {
                _program.HandleFinish("Back-test completed");
            }
        }
    }
}