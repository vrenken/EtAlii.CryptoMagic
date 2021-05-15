namespace EtAlii.BinanceMagic.Service
{
    using System.Collections.Generic;

    public class CircularExperiment : Experiment
    {
        public IList<CircularTradeDetailsSnapshot> Snapshots { get; private set; } = new List<CircularTradeDetailsSnapshot>();
    }
}