namespace EtAlii.BinanceMagic.Circular
{
    using System.Collections.Generic;
    using System.Threading;

    public interface IData
    {
        IReadOnlyList<TradeDetails> History { get; }
        
        void Load();
        TradeDetails FindLastPurchase(string coin);
        TradeDetails FindLastSell(string coin);
        decimal GetTotalProfits();
        bool TryGetSituation(TradeDetails status, CancellationToken cancellationToken, out Situation situation, out string error);
        void Add(TradeDetails tradeDetails);
    }
}