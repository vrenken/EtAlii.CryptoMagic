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

            var transactionCount = data.CircularTransactions
                .Include(s => s.Trading)
                .Count(s => s.Trading.Id == _context.Trading.Id);

            var source = transaction.BuySymbol ?? _context.Trading.FirstSymbol;
            var destination = transaction.SellSymbol ?? _context.Trading.SecondSymbol;

            var target = transaction.Target > 0m
                ? transaction.Target * _context.Trading.TargetIncrease
                : _context.Trading.InitialTarget;
            
            transaction.SellSymbol = source;
            transaction.BuySymbol = destination;
            transaction.Step = transactionCount + 1;
            transaction.Target = target;
            
            transaction.Result = "Found next target";
        }
    }
}