namespace EtAlii.BinanceMagic.Service
{
    using Blazorise;

    public static class DefaultLayout
    {
        public static IFluentSpacingOnBreakpointWithSideAndSize Margin => new FluentMargin().Is1;
        
        
        public static IFluentColumnWithSize ItemColumnWidth => new FluentColumn().IsFull.OnMobile.IsHalf.OnTablet;

        public static IFluentColumnWithSize ItemDashboardColumnWidth => new FluentColumn().IsFull.OnMobile.IsHalf.OnTablet.Is4.OnDesktop;
    }
}