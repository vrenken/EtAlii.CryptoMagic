namespace EtAlii.CryptoMagic
{
    using System;

    public partial class ExperimentalList 
    {
        protected override string GetViewNavigationUrl(Guid id) => $"/experimental/view/{id}";
        protected override string GetEditNavigationUrl() => "/experimental/edit";
    }
}