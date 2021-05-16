namespace EtAlii.BinanceMagic.Service.Trading
{
    using System;
    using System.Linq;
    using EtAlii.BinanceMagic.Service.Trading.Simple;
    using Microsoft.EntityFrameworkCore;

    public partial class AlgorithmManager 
    {
        public void Update(TradingBase trading)
        {
            var isNewTrading = trading.Id == Guid.Empty;
            var data = new DataContext();

            data.Entry(trading).State = isNewTrading
                ? EntityState.Added
                : EntityState.Modified;

            data.SaveChanges();

            if (!isNewTrading)
            {
                var runnerToReplace = Runners.Single(r => r.Trading.Id == trading.Id);
                runnerToReplace.StopAsync(_cancellationToken);
                Runners.Remove(runnerToReplace);
            }
            
            var runner = CreateRunner(trading);
            Runners.Add(runner);
        }

        private IAlgorithmRunner CreateRunner(TradingBase trading)
        {
            return new SimpleAlgorithmRunner(trading);
        }
    }
}