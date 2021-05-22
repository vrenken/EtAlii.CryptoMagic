namespace EtAlii.BinanceMagic.Service
{
    public interface IAlgorithmRunner
    {
        void Start();
        void Stop();
        TradingBase Trading { get; }
    }
    
    public interface IAlgorithmRunner<out TTrading> : IAlgorithmRunner
        where TTrading: TradingBase
    {
        event System.Action Changed;
        string Log { get; }
        new TTrading Trading { get; }
        
        TradingBase IAlgorithmRunner.Trading => Trading;
    }
}