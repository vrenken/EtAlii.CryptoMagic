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
        
        public bool TryGetTrend(string coin, string referenceCoin, CancellationToken cancellationToken, out decimal trend, out string error)
        {
            var coinComparedToReference = $"{coin}{referenceCoin}"; 
            var result = _client.Spot.Market.GetKlines(coinComparedToReference, KlineInterval.FiveMinutes, limit:1 , ct: cancellationToken);
            if (result.Error != null)
            {
                error = $"Failure fetching candlestick data for {coin}: {result.Error}";
                trend = 0m;
                return false;
            }

            var data = result.Data.Single();
            trend = data.Close - data.Open;
            error = null;
            return true;
        }
        
        public bool TryGetTrends(string[] coins, string referenceCoin, int period, CancellationToken cancellationToken, out Trend[] trends, out string error)
        {
            var result = new List<Trend>();
            foreach (var coin in coins)
            {
                var coinComparedToReference = $"{coin}{referenceCoin}"; 
                var response = _client.Spot.Market.GetKlines(coinComparedToReference, KlineInterval.OneMinute, limit:period * 2, ct: cancellationToken);
                if (response.Error != null)
                {
                    error = $"Failure fetching candlestick data for {coin}: {response.Error}";
                    trends = null;
                    return false;
                }

                var priceResponse = _client.Spot.Market.GetPrice(coinComparedToReference, cancellationToken);
                if (response.Error != null)
                {
                    error = $"Failure fetching price data for {coin}: {response.Error}";
                    trends = null;
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
                var rsiSequence = candles.StochRsi(period).ToArray();
                var rsi = rsiSequence.Last().Tick;

                var data = response.Data.Last();
                result.Add(new Trend
                {
                    Coin = coin,
                    Open = data.Open,
                    High = data.High,
                    Low = data.Low,
                    Close = data.Close,
                    //Change = data.Close - data.Open,
                    Rsi = rsi!.Value,
                    Price = priceResponse.Data.Price
                });
            }

            trends = result.ToArray();
            error = null;
            return true;
        }
    }
}