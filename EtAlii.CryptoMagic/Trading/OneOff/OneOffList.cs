namespace EtAlii.CryptoMagic
{
    using System;
    using System.Linq;
    using Blazorise;
    using Microsoft.EntityFrameworkCore;

    public partial class OneOffList 
    {
        protected override string GetViewNavigationUrl(Guid id) => $"/one-off/list";
        protected override string GetEditNavigationUrl() => "/one-off/edit";

        private IFluentSizing ButtonWidth { get; } = new FluentSizing(SizingType.Width).WithSize(SizingSize.Is100);
        private IFluentSizing ButtonHeight { get; } = new FluentSizing(SizingType.Height).WithSize(SizingSize.Is100);

        private int _totalSuccesses;
        private int _totalFailure;
        private int _totalOpen;
        private decimal _totalCurrentProfit;
        private decimal _totalGuaranteedProfit;
        private decimal _referenceSymbolBalance;

        protected override void OnRunnerChanged(IAlgorithmRunner<OneOffTransaction, OneOffTrading> runner)
        {
            base.OnRunnerChanged(runner);

            InvokeAsync(async () =>
            {
                await using var data = new DataContext();
                var oneOffTradings = await data.OneOffTradings.ToArrayAsync();
                _totalSuccesses = oneOffTradings.Count(t => t.IsSuccess);
                _totalFailure = oneOffTradings.Count(t => t.IsCancelled);
                _totalOpen = oneOffTradings.Count(t => !t.IsCancelled && !t.IsSuccess);

                _totalCurrentProfit = oneOffTradings
                    .Sum(t => t.FinalQuoteQuantity - t.PurchaseQuoteQuantity);
                _totalGuaranteedProfit = oneOffTradings
                    .Where(t => t.IsCancelled || t.IsSuccess)
                    .Sum(t => t.FinalQuoteQuantity - t.PurchaseQuoteQuantity);
                StateHasChanged();
            });
        }

        private void UpdateReferenceSymbolBalance()
        {
            InvokeAsync(async () =>
            {
                _referenceSymbolBalance = await ApplicationContext.LiveClient
                    .GetBalance(ApplicationContext.ReferenceSymbol)
                    .ConfigureAwait(true);
                StateHasChanged();
            });
        }

        protected override void OnLocationChanged()
        {
            base.OnLocationChanged();
            UpdateReferenceSymbolBalance();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            UpdateReferenceSymbolBalance();
        }

        protected override bool Filter(IAlgorithmRunner<OneOffTransaction, OneOffTrading> runner)
        {
            var trading = runner.Context.Trading;
            return !trading.IsCancelled && !trading.IsSuccess;
        }

        private string ToTime(TimeSpan? ts)
        {
            return ts switch
            {
                _ when !ts.HasValue => "Unknown",
                _ when Math.Abs(ts.Value.TotalDays - 1) < 0.0001f => $"{ts.Value.TotalDays:0} day",
                _ when ts.Value.TotalDays > 1 => $"{ts.Value.TotalDays:0} days",
                _ when Math.Abs(ts.Value.TotalHours - 1) < 0.0001f => $"{ts.Value.TotalHours:0} hour",
                _ when ts.Value.TotalHours > 1 => $"{ts.Value.TotalHours:0} hours",
                _ when Math.Abs(ts.Value.TotalMinutes - 1) < 0.0001f => $"{ts.Value.TotalMinutes:0} minute",
                _ => $"{ts.Value.TotalMinutes:0} minutes",
            };
        }

        private decimal ToCurrentValue(OneOffTrading trading) => trading.PurchaseSymbolQuantity * trading.CurrentPrice;

        private string ToPercentage(decimal d)
        {
            var prefix = d >= 0 ? "+" : "";
            return $"{prefix}{d:0.00}";

        }

        private string ToLossMessage(OneOffTrading trading)
        {
            var change = trading.FinalQuoteQuantity - trading.PurchaseQuoteQuantity;
            return change <= 0
                ? $"(loss= ~{Math.Abs(change):0.00} {trading.ReferenceSymbol})"
                : $"(profit= ~{Math.Abs(change):0.00} {trading.ReferenceSymbol})";
        }

        private string ToCancelConfirmationMessage(OneOffTrading trading)
        {
            if (trading == null) return "";
            var change = trading.FinalQuoteQuantity - trading.PurchaseQuoteQuantity;
            return change <= 0
                ? $"This will cause a loss of ~{Math.Abs(change):0.00} {trading.ReferenceSymbol})"
                : $"This will cause a payout of ~{trading.FinalQuoteQuantity:0.00} {trading.ReferenceSymbol}";
        }

        private Color ToButtonColor(OneOffTrading trading)
        {
            var change = trading.FinalQuoteQuantity - trading.PurchaseQuoteQuantity;
            return change <= 0
                ? Color.Danger
                : Color.Info;
        }
    }
}