namespace EtAlii.BinanceMagic
{
    using System.Linq;
    using Binance.Net.Objects.Spot.MarketData;

    public class Algorithm : IAlgorithm
    {
        private readonly LoopSettings _settings;
        private readonly IData _data;
        private readonly IProgram _program;
        private readonly TradeDetails _details;

        public Algorithm(LoopSettings settings, IData data, IProgram program, TradeDetails details)
        {
            _settings = settings;
            _data = data;
            _program = program;
            _details = details;
        }

        public bool TransactionIsWorthIt(Situation situation, out SellAction sellAction, out BuyAction buyAction)
        {
            sellAction = null;
            buyAction = null;
            
            // TODO: This _details.Goal and _details.Target don't match.
            _details.Target = _details.Goal / 10m; 
            _details.SellQuantity = situation.Source.PastQuantity * _settings.MaxQuantityToTrade;
            _details.SellPrice = situation.Source.PresentPrice * _details.SellQuantity;
            _details.BuyQuantity = _details.Goal / situation.Destination.PresentPrice * _settings.MaxQuantityToTrade;
            _details.BuyPrice = _details.BuyQuantity * situation.Destination.PresentPrice;
            _details.SufficientProfit = _details.SellPrice - _details.BuyPrice > _details.Target;
            _details.Difference = _details.SellPrice - _details.BuyPrice;

            _details.SellQuantityMinimum = GetMinimalQuantity(situation.Source.Coin, situation.ExchangeInfo, _settings);
            _details.BuyQuantityMinimum = GetMinimalQuantity(situation.Destination.Coin, situation.ExchangeInfo, _settings);
            _details.SellPriceIsAboveNotionalMinimum = _details.SellPrice > _details.SellQuantityMinimum;
            _details.BuyPriceIsAboveNotionalMinimum = _details.BuyPrice > _details.BuyQuantityMinimum; 
            _details.IsWorthIt = _details.SufficientProfit && _details.SellPriceIsAboveNotionalMinimum && _details.BuyPriceIsAboveNotionalMinimum;

            _details.DumpToConsole();

            _data.AddTrend(_details.Target, _details.SellPrice, _details.SellQuantity, _details.BuyPrice, _details.BuyQuantity, _details.Difference);
            
            if (_details.IsWorthIt)
            {
                sellAction = new SellAction
                {
                    Coin = situation.Source.Coin,
                    Quantity = _details.SellQuantity,
                    UnitPrice = situation.Source.PresentPrice,
                    Price = _details.SellPrice,
                    TransactionId = $"{_details.TransactionId:000000}_0_{_details.FromCoin}_{_details.ToCoin}",
                };
                buyAction = new BuyAction
                {
                    Coin = situation.Destination.Coin,
                    Quantity = _details.BuyQuantity,
                    UnitPrice = situation.Destination.PresentPrice,
                    Price = _details.BuyPrice,
                    TransactionId = $"{_details.TransactionId:000000}_1_{_details.ToCoin}_{_details.FromCoin}",
                };
            }

            return _details.IsWorthIt;
        }

        public void ToInitialConversionActions(Situation situation, out SellAction sellAction, out BuyAction buyAction)
        {
            var lastPurchaseForSource = _data.FindLastPurchase(_details.FromCoin);
            var quantityToSell = lastPurchaseForSource == null
                ? (1 / situation.Source.PresentPrice) * GetMinimalQuantity(_details.FromCoin, situation.ExchangeInfo, _settings)
                : lastPurchaseForSource.Quantity;

            var quantityToBuy = (1 / situation.Destination.PresentPrice) * GetMinimalQuantity(_details.ToCoin, situation.ExchangeInfo, _settings);

            var sourcePrice = situation.Source.PresentPrice;// _client.GetPrice(target.Source, _settings.ReferenceCoin, cancellationToken);

            quantityToBuy = quantityToBuy * _settings.NotionalMinCorrection * _settings.InitialBuyFactor;
            quantityToSell = quantityToSell * _settings.NotionalMinCorrection;// * _settings.InitialSellFactor;

            var previousTransaction = _data.Transactions.LastOrDefault();
            if (previousTransaction == null)
            {
                sellAction = new SellAction
                {
                    Coin = _details.FromCoin,
                    UnitPrice = sourcePrice,
                    Quantity = quantityToSell,
                    Price = sourcePrice * quantityToSell,
                    TransactionId = $"{_details.TransactionId:000000}_0_{_details.FromCoin}_{_details.ToCoin}",
                };
            }
            else
            {
                if (previousTransaction.To.Coin != _details.FromCoin)
                {
                    _program.HandleFail($"Previous initial transaction did not purchase {_details.FromCoin}");
                }
                
                var sourceQuantityToSell = previousTransaction.To.Quantity;
                sellAction = new SellAction
                {
                    Coin = _details.FromCoin,
                    Quantity = sourceQuantityToSell,
                    UnitPrice = sourcePrice,
                    Price = sourcePrice * sourceQuantityToSell,
                    TransactionId = $"{_details.TransactionId:000000}_0_{_details.FromCoin}_{_details.ToCoin}",
                };
            }

            var destinationPrice = situation.Destination.PresentPrice;// _client.GetPrice(target.Destination, _settings.ReferenceCoin, cancellationToken);
            buyAction = new BuyAction
            {
                Coin = _details.ToCoin,
                UnitPrice = destinationPrice,
                Quantity = quantityToBuy,
                Price = destinationPrice * quantityToBuy,
                TransactionId = $"{_details.TransactionId:000000}_1_{_details.ToCoin}_{_details.FromCoin}",
            };
        }

        private decimal GetMinimalQuantity(string coin, BinanceExchangeInfo exchangeInfo, LoopSettings loopSettings)//, CancellationToken cancellationToken)
        {
            var symbol = exchangeInfo.Symbols.Single(s => s.BaseAsset == coin && s.QuoteAsset == loopSettings.ReferenceCoin);
            return symbol.MinNotionalFilter!.MinNotional;
        }
    }
}