namespace EtAlii.BinanceMagic.Service
{
    public interface ITradeDetailsUpdater
    {
        void UpdateTargetDetails(CircularTradeSnapshot snapshot);
    }
}