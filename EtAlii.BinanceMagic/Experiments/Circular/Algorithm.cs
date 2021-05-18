namespace EtAlii.BinanceMagic.Circular
{
    using System;
    using System.Linq;

    public class Algorithm : ICircularAlgorithm
    {
        private readonly AlgorithmSettings _settings;
        private readonly IData _data;
        private readonly IClient _client;
        private readonly TradeDetails _details;
        private readonly StatusProvider _statusProvider;

        public Algorithm(
            AlgorithmSettings settings, 
            IData data, 
            IClient client,
            TradeDetails details, 
            StatusProvider statusProvider)
        {
            _settings = settings;
            _data = data;
            _client = client;
            _details = details;
            _statusProvider = statusProvider;
        }

        public bool TransactionIsWorthIt(Situation situation, out SellAction sellAction, out BuyAction buyAction)
        {
            _details.SellQuantityMinimum = _client.GetMinimalQuantity(situation.Source.Coin, situation.ExchangeInfo, _settings.ReferenceCoin);
            _details.BuyQuantityMinimum = _client.GetMinimalQuantity(situation.Destination.Coin, situation.ExchangeInfo, _settings.ReferenceCoin);

            _details.SellQuantity = situation.Source.PastQuantity * _settings.MaxQuantityToTrade;
            _details.SellPrice = situation.Source.PresentPrice * _details.SellQuantity;
            _details.SellTrend = situation.SellTrend;
            _details.BuyQuantity = ((_details.BuyQuantityMinimum * _settings.QuantityFactor) / situation.Destination.PresentPrice) * _settings.MaxQuantityToTrade;
            _details.BuyPrice = _details.BuyQuantity * situation.Destination.PresentPrice;
            _details.BuyTrend = situation.BuyTrend;
            _details.SufficientProfit = _details.SellPrice - _details.BuyPrice > _details.Target;
            _details.Difference = _details.SellPrice - _details.BuyPrice;

            _details.SellPriceIsAboveNotionalMinimum = _details.SellPrice > _details.SellQuantityMinimum;
            _details.BuyPriceIsAboveNotionalMinimum = _details.BuyPrice > _details.BuyQuantityMinimum;

            _details.TrendsAreNegative = _details.SellTrend < 0 || _details.BuyTrend > 0;
            _details.IsWorthIt = _details.SufficientProfit && _details.SellPriceIsAboveNotionalMinimum && _details.BuyPriceIsAboveNotionalMinimum && _details.TrendsAreNegative;

            _statusProvider.RaiseChanged();

            if (_details.IsWorthIt)
            {
                sellAction = new SellAction
                {
                    Coin = situation.Source.Coin,
                    Quantity = _details.SellQuantity,
                    UnitPrice = situation.Source.PresentPrice,
                    Price = _details.SellPrice,
                    TransactionId = $"{_details.Step:000000}_0_{_details.SellCoin}_{_details.BuyCoin}",
                };
                buyAction = new BuyAction
                {
                    Coin = situation.Destination.Coin,
                    Quantity = _details.BuyQuantity,
                    UnitPrice = situation.Destination.PresentPrice,
                    Price = _details.BuyPrice,
                    TransactionId = $"{_details.Step:000000}_1_{_details.BuyCoin}_{_details.SellCoin}",
                };
            }
            else
            {
                sellAction = null;
                buyAction = null;
            }

            return _details.IsWorthIt;
        }

        public void ToInitialConversionActions(Situation situation, out SellAction sellAction, out BuyAction buyAction)
        {
            var lastPurchaseForSource = _data.FindLastPurchase(_details.SellCoin);
            var quantityToSell = 
                lastPurchaseForSource?.BuyQuantity ?? 
                (1 / situation.Source.PresentPrice) * _client.GetMinimalQuantity(_details.SellCoin, situation.ExchangeInfo, _settings.ReferenceCoin);

            var quantityToBuy = (1 / situation.Destination.PresentPrice) * _client.GetMinimalQuantity(_details.BuyCoin, situation.ExchangeInfo, _settings.ReferenceCoin);

            var sourcePrice = situation.Source.PresentPrice;// _client.GetPrice(target.Source, _settings.ReferenceCoin, cancellationToken);

            quantityToBuy = quantityToBuy * _settings.NotionalMinCorrection * _settings.QuantityFactor;
            quantityToSell = quantityToSell * _settings.NotionalMinCorrection * _settings.QuantityFactor;

            var previousTransaction = _data.History.LastOrDefault();
            if (previousTransaction == null)
            {
                sellAction = new SellAction
                {
                    Coin = _details.SellCoin,
                    UnitPrice = sourcePrice,
                    Quantity = quantityToSell,
                    Price = sourcePrice * quantityToSell,
                    TransactionId = $"{_details.Step:000000}_0_{_details.SellCoin}_{_details.BuyCoin}",
                };
            }
            else
            {
                if (previousTransaction.BuyCoin != _details.SellCoin)
                {
                    var message = $"Previous initial transaction did not purchase {_details.SellCoin}";
                    throw new InvalidOperationException(message);
                }
                
                var sourceQuantityToSell = previousTransaction.BuyQuantity;
                sellAction = new SellAction
                {
                    Coin = _details.SellCoin,
                    Quantity = sourceQuantityToSell,
                    UnitPrice = sourcePrice,
                    Price = sourcePrice * sourceQuantityToSell,
                    TransactionId = $"{_details.Step:000000}_0_{_details.SellCoin}_{_details.BuyCoin}",
                };
            }

            var destinationPrice = situation.Destination.PresentPrice;
            buyAction = new BuyAction
            {
                Coin = _details.BuyCoin,
                UnitPrice = destinationPrice,
                Quantity = quantityToBuy,
                Price = destinationPrice * quantityToBuy,
                TransactionId = $"{_details.Step:000000}_1_{_details.BuyCoin}_{_details.SellCoin}",
            };
        }
    }
}