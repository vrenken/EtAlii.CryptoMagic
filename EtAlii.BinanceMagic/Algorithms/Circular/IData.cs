namespace EtAlii.BinanceMagic.Circular
{
    using System.Collections.Generic;
    using System.Threading;

    public interface IData
    {
        IReadOnlyList<Transaction> Transactions { get; }
        
        void Load();
        Coin FindLastPurchase(string coin);
        Coin FindLastSell(string coin);
        decimal GetTotalProfits();
        bool TryGetSituation(TradeDetails status, CancellationToken cancellationToken, out Situation situation, out string error);
        void AddTransaction(Transaction transaction);
    }
}