namespace EtAlii.BinanceMagic.Service
{
    using System;

    public class CircularAlgorithm : ICircularAlgorithm
    {
        private readonly IClient _client;
        private readonly IAlgorithmContext<CircularTradeSnapshot, CircularTrading> _context;

        public CircularAlgorithm(
            IClient client,
            IAlgorithmContext<CircularTradeSnapshot, CircularTrading> context)
        {
            _client = client;
            _context = context;
        }

        public bool TransactionIsWorthIt(Situation situation, out SellAction sellAction, out BuyAction buyAction)
        {
            var snapshot = _context.Snapshot;
            snapshot.SellQuantityMinimum = _client.GetMinimalQuantity(situation.Source.Symbol, situation.ExchangeInfo, _context.Trading.ReferenceSymbol);
            snapshot.SellQuantity = situation.Source.PastQuantity * _context.Trading.MaxQuantityToTrade;
            snapshot.SellPrice = situation.Source.PresentPrice * snapshot.SellQuantity;
            snapshot.SellPriceIsOptimal = snapshot.SellPrice >= snapshot.SellQuantityMinimum;
            snapshot.SellTrend = situation.SellTrend;
            snapshot.SellTrendIsOptimal = snapshot.SellPrice <= 60m; // Remark. it is positive when it does not increase anymore.

            snapshot.BuyQuantityMinimum = _client.GetMinimalQuantity(situation.Destination.Symbol, situation.ExchangeInfo, _context.Trading.ReferenceSymbol);
            snapshot.BuyQuantity = ((snapshot.BuyQuantityMinimum * _context.Trading.QuantityFactor) / situation.Destination.PresentPrice) * _context.Trading.MaxQuantityToTrade;
            snapshot.BuyPrice = snapshot.BuyQuantity * situation.Destination.PresentPrice;
            snapshot.BuyPriceIsOptimal = snapshot.BuyPrice >= snapshot.BuyQuantityMinimum;
            snapshot.BuyTrend = situation.BuyTrend;
            snapshot.BuyTrendIsOptimal = snapshot.BuyTrend >= 40m; // Remark. it is positive when it does not decrease anymore. 

            snapshot.Difference = snapshot.SellPrice - snapshot.BuyPrice;
            snapshot.DifferenceIsOptimal = snapshot.Difference > snapshot.Target;
            
            snapshot.IsWorthIt = 
                snapshot.DifferenceIsOptimal && 
                snapshot.SellPriceIsOptimal && 
                snapshot.BuyPriceIsOptimal && 
                snapshot.SellTrendIsOptimal &&
                snapshot.BuyTrendIsOptimal;

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

            var lastPurchaseForSource = data.FindLastPurchase(snapshot.SellSymbol, _context.Trading);
            var quantityToSell = 
                lastPurchaseForSource?.BuyQuantity ?? 
                (1 / situation.Source.PresentPrice) * _client.GetMinimalQuantity(snapshot.SellSymbol, situation.ExchangeInfo, _context.Trading.ReferenceSymbol);

            var quantityToBuy = (1 / situation.Destination.PresentPrice) * _client.GetMinimalQuantity(snapshot.BuySymbol, situation.ExchangeInfo, _context.Trading.ReferenceSymbol);

            var sourcePrice = situation.Source.PresentPrice;// _client.GetPrice(target.Source, _settings.ReferenceCoin, cancellationToken);

            quantityToBuy = quantityToBuy * _context.Trading.NotionalMinCorrection * _context.Trading.QuantityFactor;
            quantityToSell = quantityToSell * _context.Trading.NotionalMinCorrection * _context.Trading.QuantityFactor;

            var previousSnapShot = data.FindPreviousSnapshot(_context.Trading);
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