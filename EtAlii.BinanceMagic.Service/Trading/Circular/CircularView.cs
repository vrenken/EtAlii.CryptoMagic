namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Globalization;
    using System.Linq;

    public partial class CircularView
    {
        private string LastSuccess => CurrentRunner.Status.LastSuccess != DateTime.MinValue
            ? CurrentRunner.Status.NextCheck.ToString(CultureInfo.InvariantCulture)
            : "None";
        
        private string NextCheck => CurrentRunner.Status.NextCheck != DateTime.MinValue
            ? CurrentRunner.Status.NextCheck.ToString(CultureInfo.InvariantCulture)
            : "Now";
        
        private CircularTradeSnapshot Current => CurrentRunner.Status; 
        protected override CircularTrading GetTrading(Guid id)
        {
            using var data = new DataContext();
            return data.CircularTradings.Single(t => t.Id == id);
        }

        protected override void OnRunnerChanged()
        {
            InvokeAsync(StateHasChanged);
        }

        private string Decimal(decimal d, string prefix = null)
        {
            prefix ??= (d > 0 ? "+" : "");
            return $"{prefix}{d:000.000000000}";
        }
    }
}