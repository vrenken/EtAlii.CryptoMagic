namespace EtAlii.BinanceMagic.Service
{
    using System;

    public class CircularAlgorithm : ICircularAlgorithm
    {
        private readonly IClient _client;
        private readonly CircularTrading _trading;
        private readonly IAlgorithmContext<CircularTradeSnapshot> _context;

        public CircularAlgorithm(
            IClient client,
            CircularTrading trading,
            IAlgorithmContext<CircularTradeSnapshot> context)
        {
            _client = client;
            _trading = trading;
            _context = context;
        }

        public bool TransactionIsWorthIt(Situation situation, out SellAction sellAction, out BuyAction buyAction)
        {
            var snapshot = _context.Snapshot;
            snapshot.SellQuantityMinimum = _client.GetMinimalQuantity(situation.Source.Symbol, situation.ExchangeInfo, _trading.ReferenceSymbol);
            snapshot.BuyQuantityMinimum = _client.GetMinimalQuantity(situation.Destination.Symbol, situation.ExchangeInfo, _trading.ReferenceSymbol);

            snapshot.SellQuantity = situation.Source.PastQuantity * _trading.MaxQuantityToTrade;
            snapshot.SellPrice = situation.Source.PresentPrice * snapshot.SellQuantity;
            snapshot.SellTrend = situation.SellTrend;
            snapshot.BuyQuantity = ((snapshot.BuyQuantityMinimum * _trading.QuantityFactor) / situation.Destination.PresentPrice) * _trading.MaxQuantityToTrade;
            snapshot.BuyPrice = snapshot.BuyQuantity * situation.Destination.PresentPrice;
            snapshot.BuyTrend = situation.BuyTrend;
            snapshot.SufficientProfit = snapshot.SellPrice - snapshot.BuyPrice > snapshot.Target;
            snapshot.Difference = snapshot.SellPrice - snapshot.BuyPrice;

            snapshot.SellPriceIsAboveNotionalMinimum = snapshot.SellPrice > snapshot.SellQuantityMinimum;
            snapshot.BuyPriceIsAboveNotionalMinimum = snapshot.BuyPrice > snapshot.BuyQuantityMinimum;

            snapshot.TrendsAreNegative = snapshot.SellTrend < 0 || snapshot.BuyTrend > 0;
            snapshot.IsWorthIt = snapshot.SufficientProfit && snapshot.SellPriceIsAboveNotionalMinimum && snapshot.BuyPriceIsAboveNotionalMinimum && snapshot.TrendsAreNegative;

            _context.RaiseChanged();

            if (snapshot.IsWorthIt)
            {
                sellAction = new SellAction
                {
                    Symbol = situation.Source.Symbol,
                    Quantity = snapshot.SellQuantity,
                    Price = situation.Source.PresentPrice,
                    QuotedQuantity = snapshot.SellPrice,
                    TransactionId = $"{snapshot.Step:000000}_0_{snapshot.SellSymbol}_{snapshot.BuySymbol}",
                };
                buyAction = new BuyAction
                {
                    Symbol = situation.Destination.Symbol,
                    Quantity = snapshot.BuyQuantity,
                    Price = situation.Destination.PresentPrice,
                    QuotedQuantity = snapshot.BuyPrice,
                    TransactionId = $"{snapshot.Step:000000}_1_{snapshot.BuySymbol}_{snapshot.SellSymbol}",
                };
            }
            else
            {
                sellAction = null;
                buyAction = null;
            }

            return snapshot.IsWorthIt;
        }

        public void ToInitialConversionActions(Situation situation, out SellAction sellAction, out BuyAction buyAction)
        {
            var snapshot = _context.Snapshot;

            using var data = new DataContext();

            var lastPurchaseForSource = data.FindLastPurchase(snapshot.SellSymbol, _trading);
            var quantityToSell = 
                lastPurchaseForSource?.BuyQuantity ?? 
                (1 / situation.Source.PresentPrice) * _client.GetMinimalQuantity(snapshot.SellSymbol, situation.ExchangeInfo, _trading.ReferenceSymbol);

            var quantityToBuy = (1 / situation.Destination.PresentPrice) * _client.GetMinimalQuantity(snapshot.BuySymbol, situation.ExchangeInfo, _trading.ReferenceSymbol);

            var sourcePrice = situation.Source.PresentPrice;// _client.GetPrice(target.Source, _settings.ReferenceCoin, cancellationToken);

            quantityToBuy = quantityToBuy * _trading.NotionalMinCorrection * _trading.QuantityFactor;
            quantityToSell = quantityToSell * _trading.NotionalMinCorrection * _trading.QuantityFactor;

            var previousSnapShot = data.FindPreviousSnapshot(_trading);
            if (previousSnapShot == null)
            {
                sellAction = new SellAction
                {
                    Symbol = snapshot.SellSymbol,
                    Price = sourcePrice,
                    Quantity = quantityToSell,
                    QuotedQuantity = sourcePrice * quantityToSell,
                    TransactionId = $"{snapshot.Step:000000}_0_{snapshot.SellSymbol}_{snapshot.BuySymbol}",
                };
            }
            else
            {
                if (previousSnapShot.BuySymbol != snapshot.SellSymbol)
                {
                    var message = $"Previous initial transaction did not purchase {snapshot.SellSymbol}";
                    throw new InvalidOperationException(message);
                }
                
                var sourceQuantityToSell = previousSnapShot.BuyQuantity;
                sellAction = new SellAction
                {
                    Symbol = snapshot.SellSymbol,
                    Quantity = sourceQuantityToSell,
                    Price = sourcePrice,
                    QuotedQuantity = sourcePrice * sourceQuantityToSell,
                    TransactionId = $"{snapshot.Step:000000}_0_{snapshot.SellSymbol}_{snapshot.BuySymbol}",
                };
            }

            var destinationPrice = situation.Destination.PresentPrice;
            buyAction = new BuyAction
            {
                Symbol = snapshot.BuySymbol,
                Price = destinationPrice,
                Quantity = quantityToBuy,
                QuotedQuantity = destinationPrice * quantityToBuy,
                TransactionId = $"{snapshot.Step:000000}_1_{snapshot.BuySymbol}_{snapshot.SellSymbol}",
            };
        }
    }
}