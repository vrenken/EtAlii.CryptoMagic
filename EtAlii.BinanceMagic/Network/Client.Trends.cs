namespace EtAlii.BinanceMagic
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Binance.Net.Enums;
    using EtAlii.BinanceMagic.Surfing;
    using Trady.Analysis.Extension;
    using Trady.Core;

    public partial class Client 
    {
        
        public bool TryGetTrend(string symbol, string referenceSymbol, int period, CancellationToken cancellationToken, out decimal trend, out string error)
        {
            if (!TryGetTrendInternal(symbol, referenceSymbol, period, cancellationToken, out var instance, out error))
            {
                trend = 0m;
                return false;
            }

            trend = instance.Rsi;
            error = null;
            return true;
        }

        private bool TryGetTrendInternal(string symbol, string referenceSymbol, int period, CancellationToken cancellationToken, out Trend trend, out string error)
        {
            var symbolComparedToReference = $"{symbol}{referenceSymbol}"; 
            var response = _client.Spot.Market.GetKlines(symbolComparedToReference, KlineInterval.OneMinute, limit:period * 2, ct: cancellationToken);
            if (response.Error != null)
            {
                error = $"Failure fetching candlestick data for {symbol}: {response.Error}";
                trend = null;
                return false;
            }

            var priceResponse = _client.Spot.Market.GetPrice(symbolComparedToReference, cancellationToken);
            if (response.Error != null)
            {
                error = $"Failure fetching price data for {symbol}: {response.Error}";
                trend = null;
                return false;
            }

            var candles = response.Data.Select(k => new Candle
            (
                dateTime: k.CloseTime,
                open: k.Open,
                high: k.High,
                low: k.Low,
                close: k.Close,
                volume: k.BaseVolume
            )).ToArray();
            var rsiSequence = candles.Rsi(period).ToArray();
            var rsi = rsiSequence.Last().Tick;

            var data = response.Data.Last();
            trend = new Trend
            {
                Symbol = symbol,
                Open = data.Open,
                High = data.High,
                Low = data.Low,
                Close = data.Close,
                //Change = data.Close - data.Open,
                Rsi = rsi!.Value,
                Price = priceResponse.Data.Price
            };

            error = null;
            return true;
        }

        public bool TryGetTrends(string[] symbols, string referenceSymbol, int period, CancellationToken cancellationToken, out Trend[] trends, out string error)
        {
            var result = new List<Trend>();
            foreach (var symbol in symbols)
            {
                if (!TryGetTrendInternal(symbol, referenceSymbol, period, cancellationToken, out var trend, out error))
                {
                    trends = null;
                    return false;
                }
                result.Add(trend);
            }

            trends = result.ToArray();
            error = null;
            return true;
        }
    }
}