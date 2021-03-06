namespace EtAlii.CryptoMagic
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface ISequence
    {
        Task<bool> Run(CancellationToken cancellationToken);

        Task Initialize(CancellationToken cancellationToken);
    }
}