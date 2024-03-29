﻿namespace EtAlii.CryptoMagic
{
    using System;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    public static class DataContextCircularExtensions
    {        
        public static CircularTransaction FindLastPurchase(this DataContext data, string symbol, CircularTrading trading, CircularTransaction transaction)
        {
            return data.CircularTransactions
                .Include(t => t.Trading)
                .Where(t => t.Trading.Id == trading.Id)
                .Where(t => t.Id != transaction.Id)
                .Where(t => t.BuyQuantity > 0.0m)
                .OrderBy(t => t.Step)
                .LastOrDefault(t => t.BuySymbol == symbol);
        }

        public static CircularTransaction[] FindPreviousTransactions(this DataContext data, CircularTrading trading)
        {
            return data.CircularTransactions
                .Include(t => t.Trading)
                .Where(t => t.Trading.Id == trading.Id)
                .OrderBy(t => t.Step)
                .ToArray();        }

        public static Cycle GetCycle(this DataContext data, CircularTrading trading)
        {
            var count = data.CircularTransactions
                .Include(t => t.Trading)
                .Count(t => t.Trading.Id == trading.Id);

            return count switch
            {
                0 => Cycle.BuyA,
                1 => Cycle.SellABuyB,
                var c when c % 2 == 0 => Cycle.TransferFromAToB, // even 0, 2, 4
                var c when c % 2 != 0 => Cycle.TransferFromBToA, // odd, 1, 3, 5
                _ => throw new InvalidOperationException("This should never ever happen.")
            };
        }
        public static bool IsInitialCycle(this DataContext data, CircularTrading trading)
        {
            return data.CircularTransactions
                .Include(t => t.Trading)
                .Count(t => t.Trading.Id == trading.Id) <= 2;
        }
        public static CircularTransaction FindLastSell(this DataContext data, string symbol, CircularTrading trading, CircularTransaction transaction)
        {
            return data.CircularTransactions
                .Include(t => t.Trading)
                .Where(t => t.Trading.Id == trading.Id)
                .Where(t => t.Id != transaction.Id)
                .Where(t => t.SellQuantity > 0.0m)
                .OrderBy(t => t.Step)
                .LastOrDefault(t => t.SellSymbol == symbol);
        }

        public static CircularTransaction FindPreviousTransaction(this DataContext data, CircularTrading trading)
        {
            return data.CircularTransactions
                .Include(t => t.Trading)
                .OrderBy(t => t.Step)
                .LastOrDefault(t => t.Trading.Id == trading.Id);
        }
        
        public static decimal GetTotalProfits(this DataContext data, CircularTrading trading)
        {
            return data.CircularTransactions
                .Include(t => t.Trading)
                .Where(t => t.Trading.Id == trading.Id)
                .Where(t => !t.IsInitialTransaction)
                .Where(t => t.Completed)
                .Sum(t => t.Profit);
        }
    }
}