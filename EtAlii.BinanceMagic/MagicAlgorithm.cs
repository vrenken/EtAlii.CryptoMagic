namespace EtAlii.BinanceMagic
{
    using System.Threading;

    public class MagicAlgorithm
    {
        private readonly Settings _settings;
        private readonly Client _client;

        public MagicAlgorithm(Settings settings, Client client)
        {
            _settings = settings;
            _client = client;
        }

        public bool TransactionIsWorthIt(Target target, Situation situation, out SellAction sellAction, out BuyAction buyAction)
        {
            sellAction = null;
            buyAction = null;
            return false;
        }

        public void ToInitialConversionActions(Target target, CancellationToken cancellationToken, out SellAction sellAction, out BuyAction buyAction)
        {
            var sourcePrice = _client.GetPrice(target.Source, cancellationToken);

            sellAction = new SellAction
            {
                Coin = target.Source,
                TargetPrice = sourcePrice,
                Quantity = _settings.InitialSell / sourcePrice,
                TransactionId = $"{target.TransactionId:000000}_0_{target.Source}_{target.Destination}",
            };
            var destinationPrice = _client.GetPrice(target.Destination, cancellationToken);
            buyAction = new BuyAction
            {
                Coin = target.Destination,
                TargetPrice = destinationPrice,
                Quantity = _settings.InitialPurchase / destinationPrice,
                TransactionId = $"{target.TransactionId:000000}_1_{target.Destination}_{target.Source}",
            };
        }
    }
}