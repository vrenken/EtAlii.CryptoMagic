namespace EtAlii.CryptoMagic.Service
{
    using System;

    public class CircularAlgorithm : ICircularAlgorithm
    {
        private readonly IClient _client;
        private readonly IAlgorithmContext<CircularTransaction, CircularTrading> _context;

        public CircularAlgorithm(
            IClient client,
            IAlgorithmContext<CircularTransaction, CircularTrading> context)
        {
            _client = client;
            _context = context;
        }

        public bool TransactionIsWorthIt(Situation situation, out SellAction sellAction, out BuyAction buyAction)
        {
            var transaction = _context.CurrentTransaction;
            transaction.SellQuantityMinimum = _client.GetMinimalQuantity(situation.Source.Symbol, situation.ExchangeInfo, _context.Trading.ReferenceSymbol);
            transaction.SellQuantity = situation.Source.PastQuantity * _context.Trading.MaxQuantityToTrade;
            transaction.SellPrice = situation.Source.PresentPrice * transaction.SellQuantity;
            transaction.SellPriceIsOptimal = transaction.SellPrice >= transaction.SellQuantityMinimum;
            transaction.SellTrend = situation.SellTrend;
            transaction.SellTrendIsOptimal = transaction.SellPrice <= 60m; // Remark. it is positive when it does not increase anymore.

            transaction.BuyQuantityMinimum = _client.GetMinimalQuantity(situation.Destination.Symbol, situation.ExchangeInfo, _context.Trading.ReferenceSymbol);
            transaction.BuyQuantity = transaction.BuyQuantityMinimum * _context.Trading.QuantityFactor / situation.Destination.PresentPrice * _context.Trading.MaxQuantityToTrade;
            transaction.BuyPrice = transaction.BuyQuantity * situation.Destination.PresentPrice;
            transaction.BuyPriceIsOptimal = transaction.BuyPrice >= transaction.BuyQuantityMinimum;
            transaction.BuyTrend = situation.BuyTrend;
            transaction.BuyTrendIsOptimal = transaction.BuyTrend >= 40m; // Remark. it is positive when it does not decrease anymore. 

            transaction.Difference = transaction.SellPrice - transaction.BuyPrice;
            transaction.DifferenceIsOptimal = transaction.Difference > transaction.Target;
            
            transaction.IsWorthIt = 
                transaction.DifferenceIsOptimal && 
                transaction.SellPriceIsOptimal && 
                transaction.BuyPriceIsOptimal && 
                transaction.SellTrendIsOptimal &&
                transaction.BuyTrendIsOptimal;

            _context.Update(transaction);

            if (transaction.IsWorthIt)
            {
                sellAction = new SellAction
                {
                    Symbol = situation.Source.Symbol,
                    Quantity = transaction.SellQuantity,
                    Price = situation.Source.PresentPrice,
                    QuotedQuantity = transaction.SellPrice,
                    TransactionId = $"{transaction.Step:000000}_0_{transaction.SellSymbol}_{transaction.BuySymbol}",
                };
                buyAction = new BuyAction
                {
                    Symbol = situation.Destination.Symbol,
                    Quantity = transaction.BuyQuantity,
                    Price = situation.Destination.PresentPrice,
                    QuotedQuantity = transaction.BuyPrice,
                    TransactionId = $"{transaction.Step:000000}_1_{transaction.BuySymbol}_{transaction.SellSymbol}",
                };
            }
            else
            {
                sellAction = null;
                buyAction = null;
            }

            return transaction.IsWorthIt;
        }

        public void ToInitialConversionActions(Situation situation, CircularTransaction transaction, out SellAction sellAction, out BuyAction buyAction)
        {
            using var data = new DataContext();

            var previousTransactions = data.FindPreviousTransactions(_context.Trading);
            if (previousTransactions.Length == 1)
            {
                sellAction = null;
            }
            else
            {
                if (previousTransactions[0].BuySymbol != transaction.SellSymbol)
                {
                    var message = $"Previous initial transaction did not purchase {transaction.SellSymbol}";
                    throw new InvalidOperationException(message);
                }
                
                var sourceQuantityToSell = previousTransactions[0].BuyQuantity;
                sellAction = new SellAction
                {
                    Symbol = transaction.SellSymbol,
                    Quantity = sourceQuantityToSell,
                    Price = situation.Source.PresentPrice,
                    QuotedQuantity = situation.Source.PresentPrice * sourceQuantityToSell,
                    TransactionId = $"{transaction.Step:000000}_0_{transaction.SellSymbol}_{transaction.BuySymbol}",
                };
            }

            var quantityToBuy = (1 / situation.Destination.PresentPrice) * _client.GetMinimalQuantity(transaction.BuySymbol, situation.ExchangeInfo, _context.Trading.ReferenceSymbol);
            quantityToBuy = quantityToBuy * _context.Trading.NotionalMinCorrection * _context.Trading.QuantityFactor;

            var destinationPrice = situation.Destination.PresentPrice;
            buyAction = new BuyAction
            {
                Symbol = transaction.BuySymbol,
                Price = destinationPrice,
                Quantity = quantityToBuy,
                QuotedQuantity = destinationPrice * quantityToBuy,
                TransactionId = $"{transaction.Step:000000}_1_{transaction.BuySymbol}_{transaction.SellSymbol}",
            };
        }
    }
}