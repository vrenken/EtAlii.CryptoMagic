namespace EtAlii.BinanceMagic
{
    using System.Threading;

    public interface ISequence
    {
        public IStatusProvider Status { get; }
        void Run(CancellationToken cancellationToken);

        void Initialize();
    }
}