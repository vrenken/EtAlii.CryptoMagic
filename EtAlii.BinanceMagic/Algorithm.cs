namespace EtAlii.BinanceMagic
{
    using System.Linq;
    using System.Threading;

    public class Algorithm
    {
        private readonly Client _client;
        private readonly LoopSettings _settings;
        private readonly Data _data;
        private readonly Program _program;

        public Algorithm(Client client, LoopSettings settings, Data data, Program program)
        {
            _client = client;
            _settings = settings;
            _data = data;
            _program = program;
        }

        public bool TransactionIsWorthIt(Target target, Situation situation, out SellAction sellAction, out BuyAction buyAction)
        {
            sellAction = null;
            buyAction = null;
            
            var sourceQuantityToSell = situation.Source.PastQuantity * _settings.MaxQuantityToTrade;
            var currentProfitIncrease = situation.Source.PresentPrice * sourceQuantityToSell;
            
            var maxDestinationQuantityToBuy = target.Profit / situation.Destination.PresentPrice * _settings.MaxQuantityToTrade;
            var currentProfitDecrease = maxDestinationQuantityToBuy * situation.Destination.PresentPrice;
            
            var currentProfit = currentProfitIncrease - currentProfitDecrease;
            var profitIncrease = target.Profit - target.PreviousProfit;
            var isWorthIt = currentProfit > profitIncrease;
            var profitDifference = currentProfitIncrease - currentProfitDecrease;
            ConsoleOutput.Write($"Target   : +{profitIncrease} {_settings.ReferenceCoin}");
            ConsoleOutput.Write($"Sell     : +{currentProfitIncrease} {_settings.ReferenceCoin} (= +{sourceQuantityToSell} {target.Source})");
            ConsoleOutput.Write($"Buy      : -{currentProfitDecrease} {_settings.ReferenceCoin} (= -{maxDestinationQuantityToBuy} {target.Destination})");
            var message =          $"Diff     : {profitDifference} {_settings.ReferenceCoin}";
            
            _data.AddTrend(profitIncrease, currentProfitIncrease, sourceQuantityToSell, currentProfitDecrease, maxDestinationQuantityToBuy, profitDifference);
            
            if (!isWorthIt)
            {
                ConsoleOutput.WriteNegative(message);
            }
            else
            {
                ConsoleOutput.WritePositive(message);
                sellAction = new SellAction
                {
                    Coin = situation.Source.Coin,
                    Quantity = sourceQuantityToSell,
                    UnitPrice = situation.Source.PresentPrice,
                    Price = currentProfitIncrease,
                    TransactionId = $"{target.TransactionId:000000}_0_{target.Source}_{target.Destination}",
                };
                buyAction = new BuyAction
                {
                    Coin = situation.Destination.Coin,
                    Quantity = maxDestinationQuantityToBuy,
                    UnitPrice = situation.Destination.PresentPrice,
                    Price = currentProfitDecrease,
                    TransactionId = $"{target.TransactionId:000000}_1_{target.Destination}_{target.Source}",
                };
            }

            return isWorthIt;
        }

        public void ToInitialConversionActions(Target target, Situation situation, CancellationToken cancellationToken, out SellAction sellAction, out BuyAction buyAction)
        {
            var exchangeInfo = _client.GetExchangeInfo(cancellationToken);

            var lastPurchaseForSource = _data.FindLastPurchase(target.Source);
            var quantityToSell = lastPurchaseForSource == null
                ? _client.GetMinimalQuantity2(target.Source, situation.Source, exchangeInfo, _settings)
                : lastPurchaseForSource.Quantity;

            var quantityToBuy = _client.GetMinimalQuantity2(target.Destination, situation.Destination, exchangeInfo, _settings);

            var sourcePrice = situation.Source.PresentPrice;// _client.GetPrice(target.Source, _settings.ReferenceCoin, cancellationToken);

            var previousTransaction = _data.Transactions.LastOrDefault();
            if (previousTransaction == null)
            {
                quantityToBuy = quantityToBuy * _settings.NotionalMinCorrection * _settings.InitialBuyFactor;
                quantityToSell = quantityToSell * _settings.NotionalMinCorrection;// * _settings.InitialSellFactor;

                sellAction = new SellAction
                {
                    Coin = target.Source,
                    UnitPrice = sourcePrice,
                    Quantity = quantityToSell,
                    Price = sourcePrice * quantityToSell,
                    TransactionId = $"{target.TransactionId:000000}_0_{target.Source}_{target.Destination}",
                };
            }
            else
            {
                if (previousTransaction.To.Coin != target.Source)
                {
                    _program.HandleFail($"Previous initial transaction did not purchase {target.Source}");
                }
                var sourceQuantityToSell = previousTransaction.To.Quantity;
                sellAction = new SellAction
                {
                    Coin = target.Source,
                    Quantity = sourceQuantityToSell,
                    UnitPrice = sourcePrice,
                    Price = sourcePrice * sourceQuantityToSell,
                    TransactionId = $"{target.TransactionId:000000}_0_{target.Source}_{target.Destination}",
                };
            }

            var destinationPrice = situation.Destination.PresentPrice;// _client.GetPrice(target.Destination, _settings.ReferenceCoin, cancellationToken);
            buyAction = new BuyAction
            {
                Coin = target.Destination,
                UnitPrice = destinationPrice,
                Quantity = quantityToBuy,
                Price = destinationPrice * quantityToBuy,
                TransactionId = $"{target.TransactionId:000000}_1_{target.Destination}_{target.Source}",
            };
        }
    }
}