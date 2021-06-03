namespace EtAlii.BinanceMagic.Service
{
    using Blazorise.Charts;

    public class SettableAxis
    {
        public object ScaleLabel { get; init; }
        public bool display { get; init; }
        public AxisTicks ticks { get; init; }
    }

    public class AxisTicks
    {
        public decimal min { get; set; }
        public decimal max { get; set; }
    }
    public class SettableChartDataset : LineChartDataset<LiveDataPoint>
    {
        // public string xAxisID { get; init; }
        // public string yAxisID { get; init; }
    }
}