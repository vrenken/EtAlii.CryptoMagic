namespace EtAlii.BinanceMagic
{
    using System.Collections.Generic;
    using System.Threading;

    public interface ICircularData
    {
        IReadOnlyList<Transaction> Transactions { get; }
        
        void Load();
        Coin FindLastPurchase(string coin);
        Coin FindLastSell(string coin);
        bool TryGetSituation(TradeDetails status, CancellationToken cancellationToken, out Situation situation);
        void AddTransaction(Transaction transaction);

        void AddTrend(decimal target, decimal sellPrice, decimal sellQuantity, decimal buyPrice, decimal buyQuantity, decimal difference);
    }
}