namespace EtAlii.BinanceMagic.Service.Trading.Simple
{
    using System;
    using System.Linq;

    public partial class SimpleEdit
    {
        protected override SimpleTrading GetTrading(Guid id)
        {
            using var data = new DataContext();
            return data.SimpleTradings.Single(t => t.Id == id);
        }

        protected override string GetNavigationUrl(Guid id) => $"/simple/view/{Model.Id}";
    }
}