namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    public partial class CircularView
    {
        private ObservableCollection<CircularTransaction> History { get; } = new();

        private CircularTransaction Current => CurrentRunner.Context.CurrentTransaction; 
        protected override CircularTrading GetTrading(Guid id)
        {
            using var data = new DataContext();
            return data.CircularTradings.SingleOrDefault(t => t.Id == id);
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
                .Where(s => s.Completed)
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