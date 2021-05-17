namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    public partial class AlgorithmManager 
    {
        public void Update(TradingBase trading)
        {
            var isNewTrading = trading.Id == Guid.Empty;
            using var data = new DataContext();

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
            return trading switch
            {
                CircularTrading circularTrading => new CircularAlgorithmRunner(circularTrading),
                SimpleTrading simpleTrading => new SimpleAlgorithmRunner(simpleTrading),
                ExperimentalTrading experimentalTrading => new ExperimentalAlgorithmRunner(experimentalTrading),
                OneOffTrading oneOffTrading => new OneOffAlgorithmRunner(oneOffTrading),
                SurfingTrading surfingTrading => new SurfingAlgorithmRunner(surfingTrading),
                _ => throw new InvalidOperationException("Not supported trading"),
            };
        }
    }
}