namespace EtAlii.BinanceMagic
{
    using System;
    using System.Linq;
    using System.Threading;
    using EtAlii.BinanceMagic.Surfing;
    using Trady.Analysis.Extension;
    using Trady.Core;

    public partial class BackTestClient 
    {
        public bool TryGetTrends(string[] symbols, string referenceSymbol, int period, CancellationToken cancellationToken, out Trend[] trends, out string error)
        {
            throw new NotSupportedException();
        }
        
        public bool TryGetTrend(string symbol, string referenceSymbol, int period, CancellationToken cancellationToken, out decimal trend, out string error)
        {
            var historyKvp = _history.Single(h => h.Key == symbol);

            var history = historyKvp.Value.SingleOrDefault(entry => entry.From < Moment && Moment <= entry.To);
            if (history == null)
            {
                trend = 0m;
                error = "No history";
                return false;
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
            trend = rsiSequence.Last().Tick ?? 0m;
            error = null;
            return true;
        }

    }
}