namespace EtAlii.BinanceMagic.Service
{
    using System;

    public interface IAlgorithmRunner
    {
        void Start();
        void Stop();
        Guid TradingId { get; }
    }
    
    public interface IAlgorithmRunner<TSnapshot, TTrading> : IAlgorithmRunner
        where TSnapshot : class
        where TTrading: TradingBase
    {
        event Action Changed;
        string Log { get; }

        IAlgorithmContext<TSnapshot, TTrading> Context { get; }
        Guid IAlgorithmRunner.TradingId => Context.Trading.Id;
    }
}