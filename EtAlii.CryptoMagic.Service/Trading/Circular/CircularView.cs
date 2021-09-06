namespace EtAlii.CryptoMagic.Service
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

        protected override void OnLocationChanged()
        {
            UpdateHistory(true);

            PopulateSnapshots();
            _lineChartOptions = CreateLineChartOptions();
            _chartStreamingOptions = CreateChartStreamingOptions();
        }

        protected override string GetListUrl() => "/circular/list";

        private void UpdateHistory(bool reset)
        {
            if (reset)
            {
                History.Clear();
            }

            if (CurrentRunner?.Context?.CurrentTransaction == null)
            {
                return;
            }

            var currentTradingId = CurrentRunner.Context.Trading.Id;
            var currentTransactionId = CurrentRunner.Context.CurrentTransaction.Id;
            using var data = new DataContext();
            var transactions = data.CircularTransactions
                .Include(s => s.Trading)
                .Where(s => s.Trading.Id == currentTradingId)
                .Where(s => s.Id != currentTransactionId)
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