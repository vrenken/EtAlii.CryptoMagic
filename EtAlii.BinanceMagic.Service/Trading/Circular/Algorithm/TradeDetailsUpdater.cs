namespace EtAlii.BinanceMagic.Service
{
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    public class TradeDetailsUpdater : ITradeDetailsUpdater
    {
        private readonly IAlgorithmContext<CircularTradeSnapshot, CircularTrading> _context;

        public TradeDetailsUpdater(IAlgorithmContext<CircularTradeSnapshot, CircularTrading> context)
        {
            _context = context;
        }

        public void UpdateTargetDetails(CircularTradeSnapshot snapshot)
        {
            using var data = new DataContext();
            var lastTransaction = data.FindPreviousSnapshot(_context.Trading);

            var snapshotCount = data.CircularTradeSnapshots
                .Include(s => s.Trading)
                .Count(s => s.Trading.Id == _context.Trading.Id);

            var source = lastTransaction == null
                ? _context.Trading.FirstSymbol
                : lastTransaction.BuySymbol;
            var destination = lastTransaction == null
                ? _context.Trading.SecondSymbol
                : lastTransaction.SellSymbol;

            var target = lastTransaction != null
                ? lastTransaction.Target * _context.Trading.TargetIncrease
                : _context.Trading.InitialTarget;
            
            snapshot.SellSymbol = source;
            snapshot.BuySymbol = destination;
            snapshot.Step = snapshotCount + 1;
            snapshot.Target = target;
            
            snapshot.Result = "Found next target";
        }
    }
}