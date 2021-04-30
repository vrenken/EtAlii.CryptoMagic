namespace EtAlii.BinanceMagic
{
    public interface IStatusProvider
    {
        event System.Action Changed;
        void Write();
    }
}