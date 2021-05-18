namespace EtAlii.BinanceMagic.Service
{
    using Microsoft.AspNetCore.Components;

    public partial class CircularTradeSnapshotCard
    {
        [Parameter] public string Title { get; set; }
        [Parameter] public CircularTradeSnapshot Snapshot { get; set; }
        
        
        private string SellTrendStyle => Snapshot.SellTrend > 0
            ? "background-color:green;color:white"
            : "background-color:red;color:white";

        private string BuyTrendStyle => Snapshot.BuyTrend < 0
            ? "background-color:green;color:white"
            : "background-color:red;color:white";

        private string DifferenceStyle => Snapshot.Difference >= Snapshot.Target
            ? "background-color:green;color:white"
            : "background-color:red;color:white";

        private string DecimalShort(decimal d)
        {
            var prefix = d >= 0 ? "+" : "";
            return $"{prefix}{d:000.000}";
        }
        private string Decimal(decimal d, string prefix = null)
        {
            prefix ??= d >= 0 ? "+" : "";
            return $"{prefix}{d:000.000000000}";
        }
    }
}