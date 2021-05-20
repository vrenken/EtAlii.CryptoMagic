namespace EtAlii.BinanceMagic.Service
{
    using Microsoft.AspNetCore.Components;

    public partial class CircularTradeSnapshotCard
    {
        [Parameter] public string Title { get; set; }
        [Parameter] public CircularTradeSnapshot Snapshot { get; set; }

        private const string PositiveStyle = "background-color:green;color:white";
        private const string NegativeStyle = "background-color:red;color:white";        
        
        private string SellPriceStyle => Snapshot.SellPriceIsPositive ? PositiveStyle : NegativeStyle;
        private string SellTrendStyle => Snapshot.SellTrendIsPositive ? PositiveStyle : NegativeStyle;

        private string BuyPriceStyle => Snapshot.BuyPriceIsPositive ? PositiveStyle : NegativeStyle;
        private string BuyTrendStyle => Snapshot.BuyTrendIsPositive ? PositiveStyle : NegativeStyle;

        private string DifferenceStyle => Snapshot.DifferenceIsPositive ? PositiveStyle : NegativeStyle;

        private string DecimalShort(decimal d)
        {
            return $"{d:000.00}";
        }
        private string Decimal(decimal d, string prefix = null)
        {
            prefix ??= d >= 0 ? "+" : "";
            return $"{prefix}{d:000.000000000}";
        }
    }
}