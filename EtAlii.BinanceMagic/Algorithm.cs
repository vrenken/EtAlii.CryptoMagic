namespace EtAlii.BinanceMagic
{
    using System.Threading;

    public class Algorithm
    {
        private readonly Client _client;
        private readonly Settings _settings;

        public Algorithm(Client client, Settings settings)
        {
            _client = client;
            _settings = settings;
        }

        public bool TransactionIsWorthIt(Target target, Situation situation, out SellAction sellAction, out BuyAction buyAction)
        {
            sellAction = null;
            buyAction = null;

            // var sourceQuantityToSell = situation.Source.PastQuantity * _settings.MaxQuantityToTrade;
            // var destinationQuantityToBuy = 0m;
            // var currentProfitIncrease = situation.Source.PresentPrice * sourceQuantityToSell;
            // var currentProfitDecrease = situation.Destination.PresentPrice * destinationQuantityToBuy;

            // var currentProfit = currentProfitIncrease - currentProfitDecrease;
            // return currentProfit > target.Profit;
            
            var sourceQuantityToSell = situation.Source.PastQuantity * _settings.MaxQuantityToTrade;
            var currentProfitIncrease = situation.Source.PresentPrice * sourceQuantityToSell;
            
            var maxDestinationQuantityToBuy = target.Profit / situation.Destination.PresentPrice * _settings.MaxQuantityToTrade;
            var currentProfitDecrease = maxDestinationQuantityToBuy * situation.Destination.PresentPrice;
            
            var currentProfit = currentProfitIncrease - currentProfitDecrease;
            var isWorthIt = currentProfit > target.Profit;
            if (isWorthIt)
            {
                sellAction = new SellAction
                {
                    Coin = situation.Source.Coin,
                    Quantity = sourceQuantityToSell,
                    UnitPrice = situation.Source.PresentPrice,
                    Price = situation.Source.PresentPrice * sourceQuantityToSell,
                    TransactionId = $"{target.TransactionId:000000}_0_{target.Source}_{target.Destination}",
                };
                buyAction = new BuyAction
                {
                    Coin = situation.Destination.Coin,
                    Quantity = maxDestinationQuantityToBuy,
                    UnitPrice = situation.Destination.PresentPrice,
                    Price = situation.Destination.PresentPrice * maxDestinationQuantityToBuy,
                    TransactionId = $"{target.TransactionId:000000}_1_{target.Destination}_{target.Source}",
                };
            }

            return isWorthIt;
        }

        public void ToInitialConversionActions(Target target, CancellationToken cancellationToken, out SellAction sellAction, out BuyAction buyAction)
        {
            var (quantityToSell, quantityToBuy) = _client.GetMinimalQuantity(target.Source, target.Destination, cancellationToken);
            var sourcePrice = _client.GetPrice(target.Source, cancellationToken);
            sellAction = new SellAction
            {
                Coin = target.Source,
                UnitPrice = sourcePrice,
                Quantity = quantityToSell,
                Price = sourcePrice * quantityToSell,
                TransactionId = $"{target.TransactionId:000000}_0_{target.Source}_{target.Destination}",
            };

            var destinationPrice = _client.GetPrice(target.Destination, cancellationToken);
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