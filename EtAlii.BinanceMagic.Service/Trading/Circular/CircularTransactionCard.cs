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
        private const string NeutralStyle = "background-color:transparent;color:black";

        private string SellPriceStyle => Transaction switch
        {
            { IsInitialTransaction: true } => NeutralStyle,
            { IsChanging: true } => NeutralStyle,
            { SellPriceIsOptimal: true } => PositiveStyle,
            { SellPriceIsOptimal: false } => NegativeStyle,
            _ => NeutralStyle
        };

        private string SellTrendStyle => Transaction switch
        {
            { IsInitialTransaction: true } => NeutralStyle,
            { IsChanging: true } => NeutralStyle,
            { SellTrendIsOptimal: true } => PositiveStyle,
            { SellTrendIsOptimal: false } => NegativeStyle,
            _ => NeutralStyle
        };

        private string BuyPriceStyle => Transaction switch
        {
            { IsInitialTransaction: true } => NeutralStyle,
            { IsChanging: true } => NeutralStyle,
            { BuyPriceIsOptimal: true } => PositiveStyle,
            { BuyPriceIsOptimal: false } => NegativeStyle,
            _ => NeutralStyle
        };

        private string BuyTrendStyle => Transaction switch
        {
            { IsInitialTransaction: true } => NeutralStyle,
            { IsChanging: true } => NeutralStyle,
            { BuyTrendIsOptimal: true } => PositiveStyle,
            { BuyTrendIsOptimal: false } => NegativeStyle,
            _ => NeutralStyle
        };

        private string DifferenceStyle => Transaction switch
        {
            { IsInitialTransaction: true } => NeutralStyle,
            { IsChanging: true } => NeutralStyle,
            { DifferenceIsOptimal: true } => PositiveStyle,
            { DifferenceIsOptimal: false } => NegativeStyle,
            _ => NeutralStyle
        };
    }
}