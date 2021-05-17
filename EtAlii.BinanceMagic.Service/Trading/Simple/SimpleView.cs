namespace EtAlii.BinanceMagic.Service.Trading.Simple
{
    using System;
    using System.Linq;

    public partial class SimpleView
    {
        public SimpleTradeDetailsSnapshot Current { get; } = new ();
        protected override SimpleTrading GetTrading(Guid id)
        {
            using var data = new DataContext();
            return data.SimpleTradings.Single(t => t.Id == id);
        }
    }
}