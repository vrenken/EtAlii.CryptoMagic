namespace EtAlii.BinanceMagic
{
    using System.Threading;
    using System.Threading.Tasks;

    public class Loop
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private Task _task;
        
        private readonly ISequence _sequence;

        public Loop(ISequence sequence)
        {
            _sequence = sequence;
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

            _sequence.Initialize(cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                _sequence.Run(cancellationToken);
            }
        }
    }
}