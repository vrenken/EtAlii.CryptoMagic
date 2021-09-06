namespace EtAlii.CryptoMagic
{
    using Blazorise.Charts;

    public class SettableAxis
    {
        public object ScaleLabel { get; init; }
        public bool Display { get; init; }
        public AxisTicks Ticks { get; init; }
    }

    public class AxisTicks
    {
        public decimal Min { get; set; }
        public decimal Max { get; set; }
    }
    public class SettableChartDataset : LineChartDataset<LiveDataPoint>
    {
        // public string xAxisID { get; init; }
        // public string yAxisID { get; init; }
    }
}