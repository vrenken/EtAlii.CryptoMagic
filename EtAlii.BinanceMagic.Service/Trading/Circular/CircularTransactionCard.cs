namespace EtAlii.BinanceMagic.Service
{
    using Microsoft.AspNetCore.Components;

    public partial class CircularTransactionCard
    {
        [Parameter] public string Title { get; set; }
        [Parameter] public CircularTransaction Transaction { get; set; }
        [Parameter] public CircularTrading Trading { get; set; }

        private const string PositiveStyle = "background-color:green;color:white";
        private const string NegativeStyle = "background-color:red;color:white";        
        
        private string SellPriceStyle => Transaction.SellPriceIsOptimal ? PositiveStyle : NegativeStyle;
        private string SellTrendStyle => Transaction.SellTrendIsOptimal ? PositiveStyle : NegativeStyle;

        private string BuyPriceStyle => Transaction.BuyPriceIsOptimal ? PositiveStyle : NegativeStyle;
        private string BuyTrendStyle => Transaction.BuyTrendIsOptimal ? PositiveStyle : NegativeStyle;

        private string DifferenceStyle => Transaction.DifferenceIsOptimal ? PositiveStyle : NegativeStyle;
    }
}