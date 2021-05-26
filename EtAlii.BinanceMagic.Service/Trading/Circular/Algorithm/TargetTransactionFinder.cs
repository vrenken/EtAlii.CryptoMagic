namespace EtAlii.BinanceMagic.Service
{
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    public class TargetTransactionFinder : ITargetTransactionFinder
    {
        private readonly IAlgorithmContext<CircularTransaction, CircularTrading> _context;

        public TargetTransactionFinder(IAlgorithmContext<CircularTransaction, CircularTrading> context)
        {
            _context = context;
        }
        
        public CircularTransaction Find()
        {
            using var data = new DataContext();

            var transaction = _context.CurrentTransaction;
            
            if (transaction == null)
            {
                transaction = data.FindPreviousTransaction(_context.Trading);
                transaction ??= new CircularTransaction
                {
                    Trading = _context.Trading,
                    SellSymbol =  _context.Trading.FirstSymbol,
                    BuySymbol = _context.Trading.SecondSymbol
                };

                if (transaction.Completed)
                {
                    transaction.SellSymbol = transaction.BuySymbol;
                    transaction.BuySymbol = transaction.SellSymbol;
                }
            }
            else
            {
                var transactionCount = data.CircularTransactions
                    .Include(s => s.Trading)
                    .Count(s => s.Trading.Id == _context.Trading.Id);    
                transaction.Step = transactionCount + 1;
                transaction.SellSymbol = transaction.BuySymbol;
                transaction.BuySymbol = transaction.SellSymbol;
            }
            
            transaction.Target = transaction.Target > 0m
                ? transaction.Target * _context.Trading.TargetIncrease
                : _context.Trading.InitialTarget;
            transaction.Result = "Found next target";
            
            return transaction;
        }
    }
}