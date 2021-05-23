namespace EtAlii.BinanceMagic.Service
{
    public interface IAlgorithmContext<TTransaction, TTrading>
        where TTransaction: TransactionBase<TTrading>
        where TTrading: TradingBase
    {
        TTrading Trading { get; }
        TTransaction CurrentTransaction { get; }
        event System.Action<AlgorithmChange> Changed;

        void Update(TTrading trading, TTransaction transaction);
        void Update(TTransaction transaction) => Update(Trading, transaction);
    }
}