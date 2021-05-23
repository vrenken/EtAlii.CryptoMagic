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
                var runnerToReplace = Runners.Single(r => r.TradingId == trading.Id);
                _runners.Remove(runnerToReplace);
            }
            
            var runner = CreateRunner(trading);
            _runners.Add(runner);
        }

        private IAlgorithmRunner CreateRunner(TradingBase trading)
        {
            trading.Start = DateTime.Now;
            return trading switch
            {
                CircularTrading circularTrading => new CircularAlgorithmRunner(circularTrading, _applicationContext),
                SimpleTrading simpleTrading => new SimpleAlgorithmRunner(simpleTrading),
                ExperimentalTrading experimentalTrading => new ExperimentalAlgorithmRunner(experimentalTrading),
                OneOffTrading oneOffTrading => new OneOffAlgorithmRunner(oneOffTrading),
                SurfingTrading surfingTrading => new SurfingAlgorithmRunner(surfingTrading),
                _ => throw new InvalidOperationException("Not supported trading"),
            };
        }
    }
}