namespace EtAlii.BinanceMagic.Service
{
    public interface IAlgorithmRunner
    {
        string Log { get; }
        TradingBase Trading { get; }
        void Start();

        void Stop();
    }
}