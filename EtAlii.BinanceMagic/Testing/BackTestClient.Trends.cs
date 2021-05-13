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
        public bool TryGetTrends(string[] coin, string referenceCoin, int period, CancellationToken cancellationToken, out Trend[] trends, out string error)
        {
            throw new NotSupportedException();
        }
        
        public bool TryGetTrend(string coin, string referenceCoin, CancellationToken cancellationToken, out decimal trend, out string error)
        {
            var period = 6;
            var historyKvp = _history.Single(h => h.Key == coin);

            var history = historyKvp.Value.SingleOrDefault(entry => entry.From < Moment && Moment <= entry.To);
            if (history == null)
            {
                trend = 0m;
                error = "No history";
                return false;
            }

            var index = Array.IndexOf(historyKvp.Value, history);

            var candles = historyKvp.Value
                .Skip(index)
                .Reverse()
                .Take(period)
                .Reverse()
                .Select(k => new Candle
            (
                dateTime: k.To,
                open: k.Open,
                high: k.High,
                low: k.Low,
                close: k.Close,
                volume: k.Volume
            )).ToArray();
            var rsiSequence = candles.StochRsi(period).ToArray();
            trend = (decimal)rsiSequence.Last().Tick!;
            error = null;
            return true;
        }

    }
}