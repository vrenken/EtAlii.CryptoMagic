namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Linq;

    public partial class SimpleView
    {
        private SimpleTransaction Current { get; } = new ();
        
        protected override string GetListUrl() => "/simple/list";

        protected override SimpleTrading GetTrading(Guid id)
        {
            using var data = new DataContext();
            return data.SimpleTradings.Single(t => t.Id == id);
        }
    }
}