namespace EtAlii.CryptoMagic
{
    public class TransactionBase<TTrading> : Entity
        where TTrading: TradingBase
    {
        public TTrading Trading { get; set; }
    }
}