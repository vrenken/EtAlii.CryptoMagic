namespace EtAlii.BinanceMagic.Service
{
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    public class TradeDetailsUpdater : ITradeDetailsUpdater
    {
        private readonly CircularTrading _trading;

        public TradeDetailsUpdater(CircularTrading trading)
        {
            _trading = trading;
        }

        public void UpdateTargetDetails(CircularTradeSnapshot snapshot)
        {
            using var data = new DataContext();
            var lastTransaction = data.FindPreviousSnapshot(_trading);

            var snapshotCount = data.CircularTradeSnapshots
                .Include(s => s.Trading)
                .Count(s => s.Trading.Id == _trading.Id);

            var source = lastTransaction == null
                ? _trading.FirstSymbol
                : lastTransaction.BuySymbol;
            var destination = lastTransaction == null
                ? _trading.SecondSymbol
                : lastTransaction.SellSymbol;

            var target = lastTransaction != null
                ? lastTransaction.Target * _trading.TargetIncrease
                : _trading.InitialTarget;
            
            snapshot.SellSymbol = source;
            snapshot.BuySymbol = destination;
            snapshot.ReferenceSymbol = _trading.ReferenceSymbol;
            snapshot.Step = snapshotCount + 1;
            snapshot.Target = target;
            
            snapshot.Result = "Found next target";
        }
    }
}