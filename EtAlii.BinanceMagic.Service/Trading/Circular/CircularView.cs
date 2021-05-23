namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    public partial class CircularView
    {
        private ObservableCollection<CircularTransaction> History { get; } = new();

        private string LastSuccess => CurrentRunner.Context.CurrentTransaction.LastSuccess != DateTime.MinValue
            ? CurrentRunner.Context.CurrentTransaction.LastSuccess.ToString(CultureInfo.CurrentCulture)
            : "None";
        
        private string NextCheck => CurrentRunner.Context.CurrentTransaction.NextCheck != DateTime.MinValue
            ? CurrentRunner.Context.CurrentTransaction.NextCheck.ToString(CultureInfo.CurrentCulture)
            : "Now";
        
        private CircularTransaction Current => CurrentRunner.Context.CurrentTransaction; 
        protected override CircularTrading GetTrading(Guid id)
        {
            using var data = new DataContext();
            return data.CircularTradings.Single(t => t.Id == id);
        }

        protected override void OnRunnerChanged() => UpdateHistory(false);

        protected override void OnLocationChanged() => UpdateHistory(true);

        private void UpdateHistory(bool reset)
        {
            if (reset)
            {
                History.Clear();
            }
            
            using var data = new DataContext();
            var transactions = data.CircularTransactions
                .Include(s => s.Trading)
                .Where(s => s.Trading.Id == CurrentRunner.Context.Trading.Id)
                .Where(s => s.IsWorthIt)
                .OrderByDescending(s => s.Step)
                .ToArray();

            var missingTransactions = transactions
                .Where(s => History.All(h => h.Step != s.Step))
                .Reverse()
                .ToArray(); 
            foreach (var transaction in missingTransactions)
            {
                History.Insert(0, transaction);
            }
        }
    }
}