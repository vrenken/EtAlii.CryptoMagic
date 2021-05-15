namespace EtAlii.BinanceMagic.Circular
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class Data : IData
    {        
        private readonly IClient _client;
        private readonly AlgorithmSettings _settings;
        private readonly IPersistence<TradeDetails> _persistence;
        public IReadOnlyList<TradeDetails> History => _persistence.Items;

        public Data(IClient client, AlgorithmSettings settings, IPersistence<TradeDetails> persistence)
        {
            _client = client;
            _settings = settings;
            _persistence = persistence;
        }

        public void Load() => _persistence.Load();
        public void Add(TradeDetails tradeDetails) => _persistence.Add(tradeDetails);
        
        public TradeDetails FindLastPurchase(string coin) => _persistence.Items.LastOrDefault(t => t.BuyCoin == coin);
        public TradeDetails FindLastSell(string coin) => _persistence.Items.LastOrDefault(t => t.SellCoin == coin);

        public decimal GetTotalProfits()
        {
            return _persistence.Items.Sum(t => t.Profit);
        }
        public bool TryGetSituation(TradeDetails details, CancellationToken cancellationToken, out Situation situation, out string error)
        {
            if (!_client.TryGetPrice(details.SellCoin, _settings.ReferenceCoin, cancellationToken, out var sourcePrice, out error))
            {
                situation = null;
                return false;
            }

            if (!_client.TryGetPrice(details.BuyCoin, _settings.ReferenceCoin, cancellationToken, out var targetPrice, out error))
            {
                situation = null;
                return false;
            }

            if (!_client.TryGetTradeFees(details.SellCoin, _settings.ReferenceCoin, cancellationToken, out var sourceMakerFee, out var _, out error))
            {
                situation = null;
                return false;
            }
            
            if (!_client.TryGetTradeFees(details.BuyCoin, _settings.ReferenceCoin, cancellationToken, out var _, out var destinationTakerFee, out error))
            {
                situation = null;
                return false;
            }

            if (!_client.TryGetTrend(details.SellCoin, _settings.ReferenceCoin, cancellationToken, out var sellTrend, out error))
            {
                situation = null;
                return false;
            }
            if (!_client.TryGetTrend(details.BuyCoin, _settings.ReferenceCoin, cancellationToken, out var buyTrend, out error))
            {
                situation = null;
                return false;
            }

            var lastSourcePurchase = FindLastPurchase(details.SellCoin);
            var sourceDelta = new Delta
            {
                Coin = details.SellCoin,
                PastPrice = lastSourcePurchase?.BuyPrice ?? sourcePrice,
                PastQuantity = lastSourcePurchase?.BuyQuantity ?? 0,
                PresentPrice = sourcePrice,
            };

            var lastTargetSell = FindLastSell(details.BuyCoin);
            var targetDelta = new Delta
            {
                Coin = details.BuyCoin,
                PastPrice = lastTargetSell?.SellPrice ?? targetPrice,
                PastQuantity = lastTargetSell?.SellQuantity ?? 0,
                PresentPrice = targetPrice
            };
            situation = new Situation
            {
                Source = sourceDelta,
                SellFee = sourceMakerFee,
                SellTrend = sellTrend,
                Destination = targetDelta,
                BuyFee = destinationTakerFee,
                BuyTrend = buyTrend,
                IsInitialCycle = lastSourcePurchase == null || lastTargetSell == null 
            };

            error = null;
            return true;
        }
    }
}