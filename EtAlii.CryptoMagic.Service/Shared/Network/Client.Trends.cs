namespace EtAlii.CryptoMagic.Service
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Binance.Net.Enums;
    using EtAlii.CryptoMagic.Service.Surfing;
    using Trady.Analysis.Extension;
    using Trady.Core;

    public partial class Client 
    {
        
        public async Task<(bool success, decimal trend, string error)> TryGetTrend(string symbol, string referenceSymbol, int period, CancellationToken cancellationToken)
        {
            var (success, instance, error) = await TryGetTrendInternal(symbol, referenceSymbol, period, cancellationToken);
            if (!success)
            {
                _log.Error(error);
                return (false, 0m, error);
            }

            return (true, instance.Rsi, null);
        }

        private async Task<(bool success, Trend trend, string error)> TryGetTrendInternal(string symbol, string referenceSymbol, int period, CancellationToken cancellationToken)
        {
            var symbolComparedToReference = $"{symbol}{referenceSymbol}"; 
            var response = await _client.Spot.Market.GetKlinesAsync(symbolComparedToReference, KlineInterval.OneMinute, limit:period * 2, ct: cancellationToken);
            if (response.Error != null)
            {
                var error = $"Failure fetching candlestick data for {symbol}: {response.Error}";
                _log.Error(error);
                return (false, null, error);
            }

            var priceResponse = await _client.Spot.Market.GetPriceAsync(symbolComparedToReference, cancellationToken);
            if (priceResponse.Error != null)
            {
                var error = $"Failure fetching price data for {symbol}: {response.Error}";
                _log.Error(error);
                return (false, null, error);
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
            var trend = new Trend
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

            return (true, trend, null);
        }

        public async Task<(bool success, Trend[] trends, string error)> TryGetTrends(string[] symbols, string referenceSymbol, int period, CancellationToken cancellationToken)
        {
            var result = new List<Trend>();
            foreach (var symbol in symbols)
            {
                var (success, trend, error) = await TryGetTrendInternal(symbol, referenceSymbol, period, cancellationToken);
                if (!success)
                {
                    _log.Error(error);
                    return (false, null, error);
                }
                result.Add(trend);
            }

            return (true, result.ToArray(), null);
        }
    }
}