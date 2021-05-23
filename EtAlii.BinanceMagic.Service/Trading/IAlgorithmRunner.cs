namespace EtAlii.BinanceMagic.Service
{
    using System;

    public interface IAlgorithmRunner
    {
        void Start();
        void Stop();
        Guid TradingId { get; }
    }
    
    public interface IAlgorithmRunner<TTransaction, TTrading> : IAlgorithmRunner
        where TTransaction : TransactionBase
        where TTrading: TradingBase
    {
        event Action Changed;
        string Log { get; }

        IAlgorithmContext<TTransaction, TTrading> Context { get; }
        Guid IAlgorithmRunner.TradingId => Context.Trading.Id;
    }
}