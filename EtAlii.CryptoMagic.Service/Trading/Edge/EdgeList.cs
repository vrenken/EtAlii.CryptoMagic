namespace EtAlii.CryptoMagic.Service
{
    using System;

    public partial class EdgeList 
    {
        protected override string GetViewNavigationUrl(Guid id) => $"/edge/view/{id}";
        protected override string GetEditNavigationUrl() => "/edge/edit";
    }
}