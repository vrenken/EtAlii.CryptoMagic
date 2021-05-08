namespace EtAlii.BinanceMagic
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Binance.Net.Enums;
    using EtAlii.BinanceMagic.Surfing;

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
        
        public bool TryGetTrends(string[] coins, string referenceCoin, CancellationToken cancellationToken, out Trend[] trends, out string error)
        {
            var result = new List<Trend>();
            foreach (var coin in coins)
            {
                var coinComparedToReference = $"{coin}{referenceCoin}"; 
                var response = _client.Spot.Market.GetKlines(coinComparedToReference, KlineInterval.FiveMinutes, limit:1 , ct: cancellationToken);
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
                
                var data = response.Data.Single();
                result.Add(new Trend
                {
                    Coin = coin,
                    Change = data.Close - data.Open,
                    Price = priceResponse.Data.Price
                });
            }

            trends = result.ToArray();
            error = null;
            return true;
        }
    }
}