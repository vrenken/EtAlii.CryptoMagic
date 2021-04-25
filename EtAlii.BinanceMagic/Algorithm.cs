﻿namespace EtAlii.BinanceMagic
{
    using System.Linq;
    using Binance.Net.Objects.Spot.MarketData;

    public class Algorithm
    {
        private readonly LoopSettings _settings;
        private readonly Data _data;
        private readonly Program _program;

        public Algorithm(LoopSettings settings, Data data, Program program)
        {
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

            var sufficientProfit = currentProfit > profitIncrease; 
            var aboveNotionalProfitIncrease = currentProfitIncrease > GetMinimalQuantity(situation.Source.Coin, situation.ExchangeInfo, _settings);
            var aboveNotionalProfitDecrease = currentProfitDecrease > GetMinimalQuantity(situation.Destination.Coin, situation.ExchangeInfo, _settings);

            var isWorthIt = sufficientProfit && aboveNotionalProfitIncrease && aboveNotionalProfitDecrease;
            
            var profitDifference = currentProfitIncrease - currentProfitDecrease;
            ConsoleOutput.Write($"Target   : +{profitIncrease} {_settings.ReferenceCoin}");
            var sellMessage = $"Sell     : +{currentProfitIncrease} {_settings.ReferenceCoin} (= +{sourceQuantityToSell} {target.Source})";
            var buyMessage =  $"Buy      : -{currentProfitDecrease} {_settings.ReferenceCoin} (= -{maxDestinationQuantityToBuy} {target.Destination})";
            var diffMessage = $"Diff     : {profitDifference} {_settings.ReferenceCoin}";

            if (aboveNotionalProfitIncrease)
            {
                ConsoleOutput.Write(sellMessage);
            }
            else
            {
                ConsoleOutput.WriteNegative(sellMessage);
            }
            if (aboveNotionalProfitDecrease)
            {
                ConsoleOutput.Write(buyMessage);
            }
            else
            {
                ConsoleOutput.WriteNegative(buyMessage);
            }
            if (sufficientProfit)
            {
                ConsoleOutput.WritePositive(diffMessage);
            }
            else
            {
                ConsoleOutput.WriteNegative(diffMessage);
            }
            
            _data.AddTrend(profitIncrease, currentProfitIncrease, sourceQuantityToSell, currentProfitDecrease, maxDestinationQuantityToBuy, profitDifference);
            
            if (isWorthIt)
            {
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