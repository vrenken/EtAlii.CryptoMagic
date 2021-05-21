namespace EtAlii.BinanceMagic.Service
{
    using System;

    public partial class OneOffList 
    {
        protected override string GetViewNavigationUrl(Guid id) => $"/one-off/view/{id}";
        protected override string GetEditNavigationUrl() => "/one-off/edit";
    }
}