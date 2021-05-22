namespace EtAlii.BinanceMagic
{
    public interface IAlgorithmContext<TSnapshot, TTrading>
        where TSnapshot: class
        where TTrading: class
    {
        TTrading Trading { get; set; }
        TSnapshot Snapshot { get; set; }
        event System.Action<AlgorithmChange> Changed;

        void RaiseChanged();
        void RaiseChanged(AlgorithmChange algorithmChange);
    }
}