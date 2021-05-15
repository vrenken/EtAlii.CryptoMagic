namespace EtAlii.BinanceMagic.Service
{
    using System.Collections.Generic;

    public class SurfingExperiment : Experiment
    {
        public IList<SurfingTradeDetailsSnapshot> Snapshots { get; private set; } = new List<SurfingTradeDetailsSnapshot>();
    }
}