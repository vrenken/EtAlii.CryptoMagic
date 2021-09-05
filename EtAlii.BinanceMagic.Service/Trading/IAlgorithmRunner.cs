namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Threading.Tasks;

    public interface IAlgorithmRunner
    {
        Task Start();
        Task Stop();
        Guid TradingId { get; }
    }
    
    public interface IAlgorithmRunner<TTransaction, TTrading> : IAlgorithmRunner
        where TTransaction : TransactionBase<TTrading>
        where TTrading: TradingBase
    {
        event Action<IAlgorithmRunner<TTransaction, TTrading>> Changed;
        string Log { get; }

        IAlgorithmContext<TTransaction, TTrading> Context { get; }
        Guid IAlgorithmRunner.TradingId => Context.Trading.Id;
    }
}