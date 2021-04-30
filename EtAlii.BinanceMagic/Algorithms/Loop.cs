namespace EtAlii.BinanceMagic
{
    using System.Threading;
    using System.Threading.Tasks;

    public class Loop
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private Task _task;
        private static readonly object LockObject = new();
        
        private readonly ISequence _sequence;
        public IStatusProvider Status { get; }
        
        public Loop(ISequence sequence)
        {
            _sequence = sequence;
            Status = _sequence.Status;
        }
        
        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _task.Wait();
        }

        public void Start()
        {
            _task = Task.Run(Run);
        }

        private void Run()
        {
            var cancellationToken = _cancellationTokenSource.Token;

            lock (LockObject)
            {
                _sequence.Initialize();
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                lock (LockObject)
                {
                    _sequence.Run(cancellationToken);
                }
            }
        }
    }
}