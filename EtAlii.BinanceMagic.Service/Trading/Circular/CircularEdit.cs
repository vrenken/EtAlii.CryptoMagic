namespace EtAlii.BinanceMagic.Service.Trading.Circular
{
    using System;
    using System.Linq;

    public partial class CircularEdit 
    {
        protected override CircularTrading GetTrading(Guid id)
        {
            using var data = new DataContext();
            return data.CircularTradings.Single(t => t.Id == id);
        }

        protected override string GetNavigationUrl(Guid id) => $"/circular/view/{Model.Id}";
    }
}