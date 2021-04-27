namespace EtAlii.BinanceMagic
{
    using System.Collections.Generic;
    using System.Threading;

    public interface IData
    {
        IReadOnlyList<Transaction> Transactions { get; }
        
        void Load();
        CoinSnapshot FindLastPurchase(string coin);
        CoinSnapshot FindLastSell(string coin);
        bool TryGetSituation(Target target, StatusInfo status, CancellationToken cancellationToken, out Situation situation);
        void AddTransaction(Transaction transaction);

        void AddTrend(decimal target, decimal sellPrice, decimal sellQuantity, decimal buyPrice, decimal buyQuantity, decimal difference);
    }
}