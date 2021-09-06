namespace EtAlii.CryptoMagic.Service
{
    using System;
    using Microsoft.AspNetCore.Components;

    public partial class Transaction
    {
        [Parameter] public string Title { get; set; }

        [Parameter] public decimal Sell { get; set; }
        [Parameter] public decimal SellQuantity { get; set; }
        [Parameter] public decimal SellTrend { get; set; }

        [Parameter] public decimal Buy { get; set; }
        [Parameter] public decimal BuyQuantity { get; set; }
        [Parameter] public decimal BuyTrend { get; set; }

        [Parameter] public decimal Diff { get; set; }


        [Parameter] public decimal Target { get; set; }

        [Parameter] public decimal Profit { get; set; }

        [Parameter] public DateTime LastSuccess { get; set; }
        [Parameter] public DateTime LastCheck { get; set; }
        [Parameter] public DateTime NextCheck { get; set; }
        [Parameter] public string Result { get; set; }
        [Parameter] public int Step { get; set; }
    }
}