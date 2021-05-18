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
        
        private string LastSuccess => CurrentRunner.Status.LastSuccess != DateTime.MinValue
            ? CurrentRunner.Status.NextCheck.ToString(CultureInfo.InvariantCulture)
            : "None";
        
        private string NextCheck => CurrentRunner.Status.NextCheck != DateTime.MinValue
            ? CurrentRunner.Status.NextCheck.ToString(CultureInfo.InvariantCulture)
            : "Now";
        
        private CircularTradeSnapshot Current => CurrentRunner.Status; 
        protected override CircularTrading GetTrading(Guid id)
        {
            using var data = new DataContext();
            return data.CircularTradings.Single(t => t.Id == id);
        }

        protected override void OnRunnerChanged() => UpdateHistory();

        protected override void OnLocationChanged() => UpdateHistory();

        private void UpdateHistory()
        {
            using var data = new DataContext();
            var snapshots = data.CircularTradeSnapshots
                .Include(s => s.Trading)
                .Where(s => s.Trading.Id == CurrentRunner.Trading.Id)
                .Where(s => s.IsWorthIt)
                .OrderByDescending(s => s.Step)
                .ToArray();

            History.Clear();
            foreach (var snapshot in snapshots)
            {
                History.Add(snapshot);
            }
        }
    }
}