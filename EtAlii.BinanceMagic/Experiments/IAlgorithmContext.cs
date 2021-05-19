namespace EtAlii.BinanceMagic
{
    public interface IAlgorithmContext<TSnapshot>
        where TSnapshot: class
    {
        TSnapshot Snapshot { get; set; }
        event System.Action<StatusInfo> Changed;

        void RaiseChanged();
        void RaiseChanged(StatusInfo statusInfo);
    }
}