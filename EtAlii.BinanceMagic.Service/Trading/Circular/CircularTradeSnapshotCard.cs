﻿namespace EtAlii.BinanceMagic.Service
{
    using Microsoft.AspNetCore.Components;

    public partial class CircularTradeSnapshotCard
    {
        [Parameter] public string Title { get; set; }
        [Parameter] public CircularTradeSnapshot Snapshot { get; set; }
        [Parameter] public CircularTrading Trading { get; set; }

        private const string PositiveStyle = "background-color:green;color:white";
        private const string NegativeStyle = "background-color:red;color:white";        
        
        private string SellPriceStyle => Snapshot.SellPriceIsOptimal ? PositiveStyle : NegativeStyle;
        private string SellTrendStyle => Snapshot.SellTrendIsOptimal ? PositiveStyle : NegativeStyle;

        private string BuyPriceStyle => Snapshot.BuyPriceIsOptimal ? PositiveStyle : NegativeStyle;
        private string BuyTrendStyle => Snapshot.BuyTrendIsOptimal ? PositiveStyle : NegativeStyle;

        private string DifferenceStyle => Snapshot.DifferenceIsOptimal ? PositiveStyle : NegativeStyle;

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