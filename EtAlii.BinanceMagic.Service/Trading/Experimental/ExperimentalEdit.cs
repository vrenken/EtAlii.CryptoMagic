namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Linq;

    public partial class ExperimentalEdit 
    {
        protected override ExperimentalTrading GetTrading(Guid id)
        {
            using var data = new DataContext();
            return data.ExperimentalTradings.Single(t => t.Id == id);
        }

        protected override string GetNavigationUrl(Guid id) => $"/experimental/view/{Model.Id}";
    }
}