namespace EtAlii.BinanceMagic
{
    using System;
    using System.Linq;
    using System.Threading;
    using Binance.Net;
    using Binance.Net.Enums;
    using Binance.Net.Objects.Spot.SpotData;

    public partial class Client 
    {
        public bool TrySell(SellAction sellAction, string referenceCoin, CancellationToken cancellationToken, Func<DateTime> getNow, out Coin coinsSold, out string error)
        {
            // var exchangeResult = _client.Spot.System.GetExchangeInfo();
            // if (!exchangeResult.Success)
            // {
            //     error = $"Failed to fetch exchange info: {exchangeResult.Error}";
            //     coinsSold = null;
            //     return false;
            // }
            // var exchangeInfo = exchangeResult.Data;

            var orderResponseType = OrderResponseType.Full;
            var timeInForce = (TimeInForce?) null;// TimeInForce.FillOrKill;
            var orderType = OrderType.Market;

            // if (!_validator.TryValidate(_client, sellAction, "Sell", referenceCoin, exchangeInfo, cancellationToken, out sellAction, out error))
            // {
            //     coinsSold = null;
            //     return false;
            // }
            
            var sellCoin = $"{sellAction.Coin}{referenceCoin}";
            // var symbolData = exchangeInfo.Symbols.SingleOrDefault(s => string.Equals(s.Name, sellCoin, StringComparison.CurrentCultureIgnoreCase));
            // var sellPrice = symbolData!.PriceFilter!.TickSize != 0
            //     ? BinanceHelpers.FloorPrice(symbolData.PriceFilter.TickSize, sellAction.Price)
            //     : sellAction.Price;

            // ReSharper disable ExpressionIsAlwaysNull
            var sellOrder = PlaceTestOrders
                ? _client.Spot.Order.PlaceTestOrder(sellCoin, OrderSide.Sell, orderType, sellAction.Quantity, null, sellAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken)
                : _client.Spot.Order.PlaceOrder(sellCoin, OrderSide.Sell, orderType, sellAction.Quantity, null, sellAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken);
            // ReSharper restore ExpressionIsAlwaysNull

            if (sellOrder.Error != null)
            {
                error = $"Failure placing sell order for {sellAction.Coin}: {sellOrder.Error}";
                coinsSold = null;
                return false;
            }

            var isSold = PlaceTestOrders
                ? sellOrder.Data.Status == OrderStatus.New
                : sellOrder.Data.Status == OrderStatus.Filled;
            if (!isSold)
            {
                error = $"Failure placing sell order for {sellAction.Coin}: {sellOrder.Data.Status}";
                coinsSold = null;
                return false;
            }

            coinsSold = new Coin
            {
                Symbol = sellAction.Coin,
                Price = sellOrder.Data.Price,
                QuoteQuantity = sellOrder.Data.QuoteQuantityFilled,
                Quantity = sellOrder.Data.QuantityFilled
            };
            error = null;
            return true;
        }

        public bool TryBuy(BuyAction buyAction, string referenceCoin, CancellationToken cancellationToken, Func<DateTime> getNow, out Coin coinsBought, out string error)
        {
            var exchangeResult = _client.Spot.System.GetExchangeInfo();
            if (!exchangeResult.Success)
            {
                error = $"Failed to fetch exchange info: {exchangeResult.Error}";
                coinsBought = null;
                return false;
            }
            var exchangeInfo = exchangeResult.Data;

            decimal testPrice = 0;
            if (PlaceTestOrders)
            {
                if (!TryGetPrice(buyAction.Coin, referenceCoin, cancellationToken, out testPrice, out error))
                {
                    coinsBought = null;
                    return false;
                }
            }

            var orderResponseType = OrderResponseType.Full;
            var timeInForce = (TimeInForce?) null;// TimeInForce.FillOrKill;
            var orderType = OrderType.Market;

            // if(!_validator.TryValidate(_client, buyAction, "Buy", referenceCoin, exchangeInfo, cancellationToken, out buyAction, out error))
            // {
            //     coinsBought = null;
            //     return false;
            // }
            
            var buyCoin = $"{buyAction.Coin}{referenceCoin}";
            var symbolData = exchangeInfo.Symbols.SingleOrDefault(s => string.Equals(s.Name, buyCoin, StringComparison.CurrentCultureIgnoreCase));
            var buyPrice = symbolData!.PriceFilter!.TickSize != 0
                ? BinanceHelpers.FloorPrice(symbolData.PriceFilter.TickSize, buyAction.Price)
                : buyAction.Price;

            // ReSharper disable ExpressionIsAlwaysNull
            var buyOrder = PlaceTestOrders
                ? _client.Spot.Order.PlaceTestOrder(buyCoin, OrderSide.Buy, orderType, null, buyPrice, buyAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken)
                : _client.Spot.Order.PlaceOrder(buyCoin, OrderSide.Buy, orderType, null, buyPrice, buyAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken);
            // ReSharper restore ExpressionIsAlwaysNull

            if (buyOrder.Error != null)
            {
                error = $"Failure placing buy order for {buyAction.Coin}: {buyOrder.Error}";
                coinsBought = null;
                return false;
            }

            var isBought = PlaceTestOrders
                ? buyOrder.Data.Status == OrderStatus.New
                : buyOrder.Data.Status == OrderStatus.Filled;
            if (!isBought)
            {
                error = $"Failure placing buy order for {buyAction.Coin}: {buyOrder.Data.Status}";
                coinsBought = null;
                return false;
            }

            coinsBought = PlaceTestOrders
                ? new Coin
                {
                    Symbol = buyAction.Coin,
                    Price  = testPrice,
                    QuoteQuantity = buyAction.Price,
                    Quantity = buyAction.Price / testPrice,
                }
                : new Coin
                {
                    Symbol = buyAction.Coin,
                    Price  = buyOrder.Data.Price,
                    QuoteQuantity = buyOrder.Data.QuoteQuantityFilled,
                    Quantity = buyOrder.Data.QuantityFilled
                };
            error = null;
            return true;
        }

        public bool TryConvert(SellAction sellAction, BuyAction buyAction, string referenceCoin, CancellationToken cancellationToken, Func<DateTime> getNow, out Transaction transaction, out string error)
        {
            var exchangeResult = _client.Spot.System.GetExchangeInfo();
            if (!exchangeResult.Success)
            {
                error = $"Failed to fetch exchange info: {exchangeResult.Error}";
                transaction = null;
                return false;
            }
            var exchangeInfo = exchangeResult.Data;

            var orderResponseType = OrderResponseType.Full;
            var timeInForce = (TimeInForce?) null;// TimeInForce.FillOrKill;
            var orderType = OrderType.Market;

            if (!_validator.TryValidate(_client, sellAction, "Sell", referenceCoin, exchangeInfo, cancellationToken, out sellAction, out error))
            {
                transaction = null;
                return false;
            }
            if(!_validator.TryValidate(_client, buyAction, "Buy", referenceCoin, exchangeInfo, cancellationToken, out buyAction, out error))
            {
                transaction = null;
                return false;
            }
            
            var sellCoin = $"{sellAction.Coin}{referenceCoin}";

            // ReSharper disable ExpressionIsAlwaysNull
            var sellOrder = PlaceTestOrders
                ? _client.Spot.Order.PlaceTestOrder(sellCoin, OrderSide.Sell, orderType, null, sellAction.Price, sellAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken)
                : _client.Spot.Order.PlaceOrder(sellCoin, OrderSide.Sell, orderType, null, sellAction.Price, sellAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken);
            // ReSharper restore ExpressionIsAlwaysNull

            if (sellOrder.Error != null)
            {
                error = $"Failure placing sell order for {sellAction.Coin}: {sellOrder.Error}";
                transaction = null;
                return false;
            }

            var isSold = PlaceTestOrders
                ? sellOrder.Data.Status == OrderStatus.New
                : sellOrder.Data.Status == OrderStatus.Filled;
            if (!isSold)
            {
                error = $"Failure placing sell order for {sellAction.Coin}: {sellOrder.Data.Status}";
                transaction = null;
                return false;
            }
            
            var buyCoin = $"{buyAction.Coin}{referenceCoin}";

            // ReSharper disable ExpressionIsAlwaysNull
            var buyOrder = PlaceTestOrders
                ? _client.Spot.Order.PlaceTestOrder(buyCoin, OrderSide.Buy, orderType, null, buyAction.Price, buyAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken)
                : _client.Spot.Order.PlaceOrder(buyCoin, OrderSide.Buy, orderType, null, buyAction.Price, buyAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken);
            // ReSharper restore ExpressionIsAlwaysNull

            if (buyOrder.Error != null)
            {
                error = $"Failure placing buy order for {buyAction.Coin}: {buyOrder.Error}";
                RollbackOrder(sellOrder.Data, cancellationToken);
                transaction = null;
                return false;
            }

            var isBought = PlaceTestOrders
                ? buyOrder.Data.Status == OrderStatus.New
                : buyOrder.Data.Status == OrderStatus.Filled;
            if (!isBought)
            {
                error = $"Failure placing buy order for {buyAction.Coin}: {buyOrder.Data.Status}";
                RollbackOrder(sellOrder.Data, cancellationToken);
                transaction = null;
                return false;
            }

            transaction = new Transaction
            {
                From = new Coin
                {
                    Symbol = sellAction.Coin,
                    Price = sellOrder.Data.Price,
                    QuoteQuantity = sellOrder.Data.QuoteQuantityFilled,
                    Quantity = sellOrder.Data.QuantityFilled
                },
                To = new Coin
                {
                    Symbol = buyAction.Coin,
                    Price = buyOrder.Data.Price,
                    QuoteQuantity = buyOrder.Data.QuoteQuantityFilled,
                    Quantity = buyOrder.Data.QuantityFilled
                },
                Moment = getNow(),
                Profit = sellOrder.Data.QuoteQuantityFilled - buyOrder.Data.QuoteQuantityFilled 
            };
            return true;
        }
        
        private void RollbackOrder(BinancePlacedOrder order, CancellationToken cancellationToken)
        {
            var cancelOrder = _client.Spot.Order.CancelOrder(order.Symbol, order.OrderId, order.ClientOrderId, ct: cancellationToken);
            if (cancelOrder.Error != null)
            {
                var message = $"Failure cancelling order for {order.ClientOrderId}: {cancelOrder.Error}";
                _program.HandleFail(message);
            }
        }
    }
}