namespace EtAlii.BinanceMagic.Service.Shared
{
    using Microsoft.AspNetCore.Components;

    public partial class Transaction
    {
        // Demonstrates how a parent component can supply parameters
        [Parameter]
        public string Title { get; set; }
    }
}