namespace EtAlii.BinanceMagic
{
    public interface IStatusProvider
    {
        event System.Action<StatusInfo> Changed;
        void Write();
    }
}