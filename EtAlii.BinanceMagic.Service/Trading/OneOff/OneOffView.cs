namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Linq;

    public partial class OneOffView
    {
        protected override OneOffTrading GetTrading(Guid id)
        {
            using var data = new DataContext();
            return data.OneOffTradings.Single(t => t.Id == id);
        }
    }
}