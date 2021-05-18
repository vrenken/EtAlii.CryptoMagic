namespace EtAlii.BinanceMagic.Service
{
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    public static class DataContextCircularExtensions
    {        
        public static CircularTradeSnapshot FindLastPurchase(this DataContext data, string symbol, CircularTrading trading)
        {
            return data.CircularTradeDetailsSnapshots
                .Include(s => s.Trading)
                .Where(s => s.Trading.Id == trading.Id)
                .Where(s => s.BuyQuantity > 0.0m)
                .OrderBy(s => s.Step)
                .LastOrDefault(s => s.BuySymbol == symbol);
        }

        public static CircularTradeSnapshot FindLastSell(this DataContext data, string symbol, CircularTrading trading)
        {
            return data.CircularTradeDetailsSnapshots
                .Include(s => s.Trading)
                .Where(s => s.Trading.Id == trading.Id)
                .Where(s => s.SellQuantity > 0.0m)
                .OrderBy(s => s.Step)
                .LastOrDefault(s => s.SellSymbol == symbol);
        }

        public static CircularTradeSnapshot FindPreviousSnapshot(this DataContext data, CircularTrading trading)
        {
            return data.CircularTradeDetailsSnapshots
                .Include(s => s.Trading)
                .OrderBy(s => s.Step)
                .LastOrDefault(s => s.Trading.Id == trading.Id);
        }
        
        public static decimal GetTotalProfits(this DataContext data, CircularTrading trading)
        {
            return data.CircularTradeDetailsSnapshots
                .Include(s => s.Trading)
                .Where(s => s.Trading.Id == trading.Id)
                .ToArray()
                .Sum(s => s.Profit);
        }
    }
}