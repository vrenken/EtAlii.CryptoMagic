namespace EtAlii.BinanceMagic.Service
{
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    public class TradeDetailsUpdater : ITradeDetailsUpdater
    {
        private readonly IAlgorithmContext<CircularTransaction, CircularTrading> _context;

        public TradeDetailsUpdater(IAlgorithmContext<CircularTransaction, CircularTrading> context)
        {
            _context = context;
        }

        public void UpdateTargetDetails(CircularTransaction transaction)
        {
            using var data = new DataContext();
            var lastTransaction = data.FindPreviousTransaction(_context.Trading);

            var transactionCount = data.CircularTransactions
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
            
            transaction.SellSymbol = source;
            transaction.BuySymbol = destination;
            transaction.Step = transactionCount + 1;
            transaction.Target = target;
            
            transaction.Result = "Found next target";
        }
    }
}