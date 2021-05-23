﻿namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    public partial class CircularView
    {
        private ObservableCollection<CircularTradeSnapshot> History { get; } = new();

        private string LastSuccess => CurrentRunner.Context.Snapshot.LastSuccess != DateTime.MinValue
            ? CurrentRunner.Context.Snapshot.LastSuccess.ToString(CultureInfo.CurrentCulture)
            : "None";
        
        private string NextCheck => CurrentRunner.Context.Snapshot.NextCheck != DateTime.MinValue
            ? CurrentRunner.Context.Snapshot.NextCheck.ToString(CultureInfo.CurrentCulture)
            : "Now";
        
        private CircularTradeSnapshot Current => CurrentRunner.Context.Snapshot; 
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
            var snapshots = data.CircularTradeSnapshots
                .Include(s => s.Trading)
                .Where(s => s.Trading.Id == CurrentRunner.Context.Trading.Id)
                .Where(s => s.IsWorthIt)
                .OrderByDescending(s => s.Step)
                .ToArray();

            var missingSnapshots = snapshots
                .Where(s => History.All(h => h.Step != s.Step))
                .Reverse()
                .ToArray(); 
            foreach (var snapshot in missingSnapshots)
            {
                History.Insert(0, snapshot);
            }
        }
    }
}