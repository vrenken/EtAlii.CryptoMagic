namespace EtAlii.BinanceMagic.Service.Data
{
    using System;

    public class Experiment
    {
        public string Name { get; init; }
        public bool IsActive { get; set; }
        public ExperimentStep[] Steps { get; init; } = Array.Empty<ExperimentStep>();
    }
}