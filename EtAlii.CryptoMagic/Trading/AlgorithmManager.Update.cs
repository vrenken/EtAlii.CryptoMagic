namespace EtAlii.CryptoMagic
{
    using System;
    using System.Linq;
    using EtAlii.CryptoMagic.Surfing;
    using Microsoft.EntityFrameworkCore;

    public partial class AlgorithmManager 
    {
        public void Remove(TradingBase trading)
        {
            var runnerToRemove = Runners.Single(r => r.TradingId == trading.Id);
            _runners.Remove(runnerToRemove);

            using var data = new DataContext();

            data.Entry(trading).State = EntityState.Deleted;

            data.SaveChanges();
        }

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
            trading.Start ??= DateTime.Now;
            
            return trading switch
            {
                CircularTrading circularTrading => new CircularAlgorithmRunner(circularTrading, _applicationContext),
                SimpleTrading simpleTrading => new SimpleAlgorithmRunner(simpleTrading),
                ExperimentalTrading experimentalTrading => new ExperimentalAlgorithmRunner(experimentalTrading),
                OneOffTrading oneOffTrading => new OneOffAlgorithmRunner(oneOffTrading),
                EdgeTrading edgeTrading => new EdgeAlgorithmRunner(edgeTrading),
                SurfingTrading surfingTrading => new SurfingAlgorithmRunner(surfingTrading),
                _ => throw new InvalidOperationException("Not supported trading"),
            };
        }
    }
}