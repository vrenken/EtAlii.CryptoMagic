namespace EtAlii.BinanceMagic.Service
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
            try
            {
                _cancellationTokenSource.Cancel();
                _task.Wait();
            }
            catch
            {
                // No operation.
            }
        }

        public void Start()
        {
            _task = Task.Run(Run);
        }

        private async Task Run()
        {
            var cancellationToken = _cancellationTokenSource.Token;

            await _sequence.Initialize(cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                var keepRunning = await _sequence.Run(cancellationToken);
                if (!keepRunning)
                {
                    break;
                }
            }
        }
    }
}