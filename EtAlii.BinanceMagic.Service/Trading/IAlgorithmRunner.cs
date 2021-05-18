namespace EtAlii.BinanceMagic.Service
{
    public interface IAlgorithmRunner
    {
        event System.Action Changed;
        string Log { get; }
        TradingBase Trading { get; }
        void Start();

        void Stop();
    }
}