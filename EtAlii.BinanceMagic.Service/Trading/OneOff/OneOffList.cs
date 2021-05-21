namespace EtAlii.BinanceMagic.Service
{
    using System;

    public partial class OneOffList 
    {
        protected override string GetViewNavigationUrl(Guid id) => $"/circular/view/{id}";
        protected override string GetEditNavigationUrl() => "/circular/edit";
    }
}