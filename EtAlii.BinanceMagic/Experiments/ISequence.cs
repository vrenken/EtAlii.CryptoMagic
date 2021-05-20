namespace EtAlii.BinanceMagic
{
    using System.Threading;

    public interface ISequence
    {
        void Run(CancellationToken cancellationToken, out bool keepRunning);

        void Initialize(CancellationToken cancellationToken);
    }
}