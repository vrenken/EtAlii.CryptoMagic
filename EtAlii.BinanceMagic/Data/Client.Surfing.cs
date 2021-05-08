namespace EtAlii.BinanceMagic
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Binance.Net.Enums;
    using EtAlii.BinanceMagic.Surfing;

    public partial class Client 
    {
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
                var data = response.Data.Single();
                result.Add(new Trend
                {
                    Coin = coin,
                    Change = data.Close - data.Open
                });
            }

            trends = result.ToArray();
            error = null;
            return true;
        }
    }
}