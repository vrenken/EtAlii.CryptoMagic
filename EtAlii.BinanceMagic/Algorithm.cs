namespace EtAlii.BinanceMagic
{
    using System.Threading;

    public class Algorithm
    {
        private readonly Client _client;

        public Algorithm(Client client)
        {
            _client = client;
        }

        public bool TransactionIsWorthIt(Target target, Situation situation, out SellAction sellAction, out BuyAction buyAction)
        {
            sellAction = null;
            buyAction = null;

            var sourceQuotaToSell = 0m;
            var destinationQuotaToBuy = 0m;
            var currentProfitIncrease = situation.Source.PresentPrice * sourceQuotaToSell;
            var currentProfitDecrease = situation.Destination.PresentPrice * destinationQuotaToBuy;
            var currentProfit = currentProfitIncrease - currentProfitDecrease;

            return currentProfit > target.Profit;
        }

        public void ToInitialConversionActions(Target target, CancellationToken cancellationToken, out SellAction sellAction, out BuyAction buyAction)
        {
            var (quantityToSell, quantityToBuy) = _client.GetMinimalQuantity(target.Source, target.Destination, cancellationToken);
            var sourcePrice = _client.GetPrice(target.Source, cancellationToken);
            sellAction = new SellAction
            {
                Coin = target.Source,
                TargetPrice = sourcePrice,
                Quantity = quantityToSell,
                TransactionId = $"{target.TransactionId:000000}_0_{target.Source}_{target.Destination}",
            };
            var destinationPrice = _client.GetPrice(target.Destination, cancellationToken);
            
            buyAction = new BuyAction
            {
                Coin = target.Destination,
                TargetPrice = destinationPrice,
                Quantity = quantityToBuy,
                TransactionId = $"{target.TransactionId:000000}_1_{target.Destination}_{target.Source}",
            };
        }
    }
}