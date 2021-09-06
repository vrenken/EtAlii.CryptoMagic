namespace EtAlii.CryptoMagic.Service
{
    using System;

    public partial class SimpleList 
    {
        protected override string GetViewNavigationUrl(Guid id) => $"/simple/view/{id}";
        protected override string GetEditNavigationUrl() => "/simple/edit";
    }
}