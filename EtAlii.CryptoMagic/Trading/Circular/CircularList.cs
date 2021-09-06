namespace EtAlii.CryptoMagic
{
    using System;

    public partial class CircularList 
    {
        protected override string GetViewNavigationUrl(Guid id) => $"/circular/view/{id}";
        protected override string GetEditNavigationUrl() => "/circular/edit";
    }
}