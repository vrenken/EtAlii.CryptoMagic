namespace EtAlii.CryptoMagic
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using EtAlii.CryptoMagic.Surfing;
    using Trady.Analysis.Extension;
    using Trady.Core;

    public partial class BackTestClient 
    {
        public Task<(bool success, Trend[] trends, string error)> TryGetTrends(string[] symbols, string referenceSymbol, int period, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<decimal> GetBalance(string symbol) => throw new NotSupportedException();

        public Task<(bool success, decimal trend, string error)> TryGetTrend(string symbol, string referenceSymbol, int period, CancellationToken cancellationToken)
        {
            var historyKvp = _history.Single(h => h.Key == symbol);

            var history = historyKvp.Value.SingleOrDefault(entry => entry.From < Moment && Moment <= entry.To);
            if (history == null)
            {
                var error = "No history";
                _log.Error(error);
                return Task.FromResult((false, 0m, error));
            }

            var index = Array.IndexOf(historyKvp.Value, history);

            var candlesToPick = period * 2;
            var candles = historyKvp.Value
                .Skip(index - candlesToPick)
                .Take(candlesToPick)
                .Select(k => new Candle
            (
                dateTime: k.To,
                open: k.Open,
                high: k.High,
                low: k.Low,
                close: k.Close,
                volume: k.Volume
            )).ToArray();
            
            var rsiSequence = candles.Rsi(period).ToArray();
            var trend = rsiSequence.Last().Tick ?? 0m;
            return Task.FromResult<(bool success, decimal trend, string)>((true, trend, null));
        }

    }
}