namespace EtAlii.BinanceMagic
{
    using System.Linq;
    using Binance.Net.Objects.Spot.MarketData;

    public class Algorithm : IAlgorithm
    {
        private readonly LoopSettings _settings;
        private readonly IData _data;
        private readonly IProgram _program;
        private readonly StatusInfo _status;

        public Algorithm(LoopSettings settings, IData data, IProgram program, StatusInfo status)
        {
            _settings = settings;
            _data = data;
            _program = program;
            _status = status;
        }

        public bool TransactionIsWorthIt(Target target, Situation situation, out SellAction sellAction, out BuyAction buyAction)
        {
            sellAction = null;
            buyAction = null;
            
            _status.Target = target.Profit / 10m;
            _status.SellQuantity = situation.Source.PastQuantity * _settings.MaxQuantityToTrade;
            _status.SellPrice = situation.Source.PresentPrice * _status.SellQuantity;
            _status.BuyQuantity = target.Profit / situation.Destination.PresentPrice * _settings.MaxQuantityToTrade;
            _status.BuyPrice = _status.BuyQuantity * situation.Destination.PresentPrice;
            _status.SufficientProfit = _status.SellPrice - _status.BuyPrice > _status.Target;
            _status.Difference = _status.SellPrice - _status.BuyPrice;

            _status.SellQuantityMinimum = GetMinimalQuantity(situation.Source.Coin, situation.ExchangeInfo, _settings);
            _status.BuyQuantityMinimum = GetMinimalQuantity(situation.Destination.Coin, situation.ExchangeInfo, _settings);
            _status.SellPriceIsAboveNotionalMinimum = _status.SellPrice > _status.SellQuantityMinimum;
            _status.BuyPriceIsAboveNotionalMinimum = _status.BuyPrice > _status.BuyQuantityMinimum; 
            _status.IsWorthIt = _status.SufficientProfit && _status.SellPriceIsAboveNotionalMinimum && _status.BuyPriceIsAboveNotionalMinimum;

            _status.DumpToConsole();

            _data.AddTrend(_status.Target, _status.SellPrice, _status.SellQuantity, _status.BuyPrice, _status.BuyQuantity, _status.Difference);
            
            if (_status.IsWorthIt)
            {
                sellAction = new SellAction
                {
                    Coin = situation.Source.Coin,
                    Quantity = _status.SellQuantity,
                    UnitPrice = situation.Source.PresentPrice,
                    Price = _status.SellPrice,
                    TransactionId = $"{target.TransactionId:000000}_0_{target.Source}_{target.Destination}",
                };
                buyAction = new BuyAction
                {
                    Coin = situation.Destination.Coin,
                    Quantity = _status.BuyQuantity,
                    UnitPrice = situation.Destination.PresentPrice,
                    Price = _status.BuyPrice,
                    TransactionId = $"{target.TransactionId:000000}_1_{target.Destination}_{target.Source}",
                };
            }

            return _status.IsWorthIt;
        }

        public void ToInitialConversionActions(Target target, Situation situation, out SellAction sellAction, out BuyAction buyAction)
        {
            var lastPurchaseForSource = _data.FindLastPurchase(target.Source);
            var quantityToSell = lastPurchaseForSource == null
                ? (1 / situation.Source.PresentPrice) * GetMinimalQuantity(target.Source, situation.ExchangeInfo, _settings)
                : lastPurchaseForSource.Quantity;

            var quantityToBuy = (1 / situation.Destination.PresentPrice) * GetMinimalQuantity(target.Destination, situation.ExchangeInfo, _settings);

            var sourcePrice = situation.Source.PresentPrice;// _client.GetPrice(target.Source, _settings.ReferenceCoin, cancellationToken);

            quantityToBuy = quantityToBuy * _settings.NotionalMinCorrection * _settings.InitialBuyFactor;
            quantityToSell = quantityToSell * _settings.NotionalMinCorrection;// * _settings.InitialSellFactor;

            var previousTransaction = _data.Transactions.LastOrDefault();
            if (previousTransaction == null)
            {
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

        private decimal GetMinimalQuantity(string coin, BinanceExchangeInfo exchangeInfo, LoopSettings loopSettings)//, CancellationToken cancellationToken)
        {
            var symbol = exchangeInfo.Symbols.Single(s => s.BaseAsset == coin && s.QuoteAsset == loopSettings.ReferenceCoin);
            return symbol.MinNotionalFilter!.MinNotional;
        }
    }
}