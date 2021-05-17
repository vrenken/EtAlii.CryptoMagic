namespace EtAlii.BinanceMagic.Service.Trading.Circular
{
    using System;
    using System.Linq;

    public partial class CircularView
    {
        public CircularTradeDetailsSnapshot Current { get; } = new ();
        protected override CircularTrading GetTrading(Guid id)
        {
            using var data = new DataContext();
            return data.CircularTradings.Single(t => t.Id == id);
        }
    }
}