namespace EtAlii.BinanceMagic
{
    using System;
    using System.Threading;

    public class BackTestTimeManager : ITimeManager
    {
        public BackTestClient Client { get; init; }
        public IOutput Output { get; init; }
        
        public bool TerminateProcessWhenCompleted { get; init; }
        
        public DateTime GetNow() => Client.Moment;

        public void Wait(TimeSpan timeSpan, CancellationToken cancellationToken)
        {
            Client.Moment += Client.Interval;

            //Task.Delay(TimeSpan.FromSeconds(10), cancellationToken).Wait(cancellationToken);
            if (Client.Moment > Client.LastRecordedHistory)
            {
                Output.WriteLine("Back-test completed");
                if (TerminateProcessWhenCompleted)
                {
                    Console.ReadLine();
                    Environment.Exit(-1);
                }
            }
        }
    }
}