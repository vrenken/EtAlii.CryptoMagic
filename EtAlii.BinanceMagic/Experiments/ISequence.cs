namespace EtAlii.BinanceMagic
{
    using System.Threading;

    public interface ISequence
    {
        void Run(CancellationToken cancellationToken);

        void Initialize(CancellationToken cancellationToken);
    }
}