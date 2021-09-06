namespace EtAlii.CryptoMagic
{
    public interface IAlgorithmContext<TTransaction, TTrading>
        where TTransaction: TransactionBase<TTrading>
        where TTrading: TradingBase
    {
        TTrading Trading { get; }
        TTransaction CurrentTransaction { get; }
        event System.Action<AlgorithmChange> Changed;

        void Update(TTransaction transaction);
    }
}