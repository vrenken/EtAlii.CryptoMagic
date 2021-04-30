namespace EtAlii.BinanceMagic
{
    using System.Linq;
    using Binance.Net.Objects.Spot.MarketData;

    public class CircularAlgorithm : ICircularAlgorithm
    {
        private readonly CircularAlgorithmSettings _settings;
        private readonly ICircularData _data;
        private readonly IProgram _program;
        private readonly TradeDetails _details;
        private readonly StatusProvider _statusProvider;

        public CircularAlgorithm(CircularAlgorithmSettings settings, ICircularData data, IProgram program, TradeDetails details, StatusProvider statusProvider)
        {
            _settings = settings;
            _data = data;
            _program = program;
            _details = details;
            _statusProvider = statusProvider;
        }

        public bool TransactionIsWorthIt(Situation situation, out SellAction sellAction, out BuyAction buyAction)
        {
            // TODO: This _details.Goal and _details.Target don't match.
            _details.Target = _details.Goal / 15m; 
            _details.SellQuantity = situation.Source.PastQuantity * _settings.MaxQuantityToTrade;
            _details.SellPrice = situation.Source.PresentPrice * _details.SellQuantity;
            _details.SellTrend = situation.SellTrend;
            _details.BuyQuantity = _details.Goal / situation.Destination.PresentPrice * _settings.MaxQuantityToTrade;
            _details.BuyPrice = _details.BuyQuantity * situation.Destination.PresentPrice;
            _details.BuyTrend = situation.BuyTrend;
            _details.SufficientProfit = _details.SellPrice - _details.BuyPrice > _details.Target;
            _details.Difference = _details.SellPrice - _details.BuyPrice;

            _details.SellQuantityMinimum = GetMinimalQuantity(situation.Source.Coin, situation.ExchangeInfo, _settings);
            _details.BuyQuantityMinimum = GetMinimalQuantity(situation.Destination.Coin, situation.ExchangeInfo, _settings);
            _details.SellPriceIsAboveNotionalMinimum = _details.SellPrice > _details.SellQuantityMinimum;
            _details.BuyPriceIsAboveNotionalMinimum = _details.BuyPrice > _details.BuyQuantityMinimum;

            _details.TrendsAreNegative = _details.SellTrend < 0 || _details.BuyTrend > 0;
            _details.IsWorthIt = _details.SufficientProfit && _details.SellPriceIsAboveNotionalMinimum && _details.BuyPriceIsAboveNotionalMinimum && _details.TrendsAreNegative;

            _statusProvider.RaiseChanged();

            _data.AddTrend(_details.Target, _details.SellPrice, _details.SellQuantity, _details.BuyPrice, _details.BuyQuantity, _details.Difference);
            
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
            var quantityToSell = lastPurchaseForSource == null
                ? (1 / situation.Source.PresentPrice) * GetMinimalQuantity(_details.SellCoin, situation.ExchangeInfo, _settings)
                : lastPurchaseForSource.Quantity;

            var quantityToBuy = (1 / situation.Destination.PresentPrice) * GetMinimalQuantity(_details.BuyCoin, situation.ExchangeInfo, _settings);

            var sourcePrice = situation.Source.PresentPrice;// _client.GetPrice(target.Source, _settings.ReferenceCoin, cancellationToken);

            quantityToBuy = quantityToBuy * _settings.NotionalMinCorrection * _settings.InitialBuyFactor;
            quantityToSell = quantityToSell * _settings.NotionalMinCorrection;// * _settings.InitialSellFactor;

            var previousTransaction = _data.Transactions.LastOrDefault();
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
                if (previousTransaction.To.Symbol != _details.SellCoin)
                {
                    _program.HandleFail($"Previous initial transaction did not purchase {_details.SellCoin}");
                }
                
                var sourceQuantityToSell = previousTransaction.To.Quantity;
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

        private decimal GetMinimalQuantity(string coin, BinanceExchangeInfo exchangeInfo, CircularAlgorithmSettings loopSettings)//, CancellationToken cancellationToken)
        {
            var symbol = exchangeInfo.Symbols.Single(s => s.BaseAsset == coin && s.QuoteAsset == loopSettings.ReferenceCoin);
            return symbol.MinNotionalFilter!.MinNotional;
        }
    }
}