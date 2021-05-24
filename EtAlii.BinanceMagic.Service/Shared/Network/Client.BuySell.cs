namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Linq;
    using System.Threading;
    using Binance.Net;
    using Binance.Net.Enums;
    using Binance.Net.Objects.Spot.SpotData;
    using Serilog;

    public partial class Client
    {
        private readonly ILogger _log = Log.ForContext<Client>();
        public bool TrySell(SellAction sellAction, string referenceSymbol, CancellationToken cancellationToken, Func<DateTime> getNow, out Symbol symbolsSold, out string error)
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
            
            var sellSymbol = $"{sellAction.Symbol}{referenceSymbol}";
            // var symbolData = exchangeInfo.Symbols.SingleOrDefault(s => string.Equals(s.Name, sellCoin, StringComparison.CurrentCultureIgnoreCase));
            // var sellPrice = symbolData!.PriceFilter!.TickSize != 0
            //     ? BinanceHelpers.FloorPrice(symbolData.PriceFilter.TickSize, sellAction.Price)
            //     : sellAction.Price;

            // ReSharper disable ExpressionIsAlwaysNull
            var sellOrder = PlaceTestOrders
                ? _client.Spot.Order.PlaceTestOrder(sellSymbol, OrderSide.Sell, orderType, sellAction.Quantity, null, sellAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken)
                : _client.Spot.Order.PlaceOrder(sellSymbol, OrderSide.Sell, orderType, sellAction.Quantity, null, sellAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken);
            // ReSharper restore ExpressionIsAlwaysNull

            if (sellOrder.Error != null)
            {
                error = $"Failure placing sell order for {sellAction.Symbol}: {sellOrder.Error}";
                _log.Error(error);
                symbolsSold = null;
                return false;
            }

            var isSold = PlaceTestOrders
                ? sellOrder.Data.Status == OrderStatus.New
                : sellOrder.Data.Status == OrderStatus.Filled;
            if (!isSold)
            {
                error = $"Failure placing sell order for {sellAction.Symbol}: {sellOrder.Data.Status}";
                _log.Error(error);
                symbolsSold = null;
                return false;
            }

            symbolsSold = new Symbol
            {
                SymbolName = sellAction.Symbol,
                Price = sellOrder.Data.Price,
                QuoteQuantity = sellOrder.Data.QuoteQuantityFilled,
                Quantity = sellOrder.Data.QuantityFilled
            };
            error = null;
            return true;
        }

        public bool TryBuy(BuyAction buyAction, string referenceSymbol, CancellationToken cancellationToken, Func<DateTime> getNow, out TradeTransaction transaction, out string error)
        {
            if (!TryBuy(buyAction, referenceSymbol, cancellationToken, getNow, out Symbol symbolsBought, out error))
            {
                transaction = null;
                return false;
            }
            transaction = new TradeTransaction
            {
                Sell = new Symbol
                {
                    SymbolName = "",
                    QuoteQuantity = 0,
                    Quantity = 0

                },
                Buy = symbolsBought,
                Moment = getNow(),
                Profit = 0 - buyAction.QuotedQuantity 
            };
            error = null;
            return true;
        }

        public bool TryBuy(BuyAction buyAction, string referenceSymbol, CancellationToken cancellationToken, Func<DateTime> getNow, out Symbol symbolsBought, out string error)
        {
            var exchangeResult = _client.Spot.System.GetExchangeInfo();
            if (!exchangeResult.Success)
            {
                error = $"Failed to fetch exchange info: {exchangeResult.Error}";
                _log.Error(error);
                symbolsBought = null;
                return false;
            }
            var exchangeInfo = exchangeResult.Data;

            decimal testPrice = 0;
            if (PlaceTestOrders)
            {
                if (!TryGetPrice(buyAction.Symbol, referenceSymbol, cancellationToken, out testPrice, out error))
                {
                    _log.Error(error);
                    symbolsBought = null;
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
            
            var buySymbol = $"{buyAction.Symbol}{referenceSymbol}";
            var symbolData = exchangeInfo.Symbols.SingleOrDefault(s => string.Equals(s.Name, buySymbol, StringComparison.CurrentCultureIgnoreCase));
            var buyPrice = symbolData!.PriceFilter!.TickSize != 0
                ? BinanceHelpers.FloorPrice(symbolData.PriceFilter.TickSize, buyAction.QuotedQuantity)
                : buyAction.QuotedQuantity;

            // ReSharper disable ExpressionIsAlwaysNull
            var buyOrder = PlaceTestOrders
                ? _client.Spot.Order.PlaceTestOrder(buySymbol, OrderSide.Buy, orderType, null, buyPrice, buyAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken)
                : _client.Spot.Order.PlaceOrder(buySymbol, OrderSide.Buy, orderType, null, buyPrice, buyAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken);
            // ReSharper restore ExpressionIsAlwaysNull

            if (buyOrder.Error != null)
            {
                error = $"Failure placing buy order for {buyAction.Symbol}: {buyOrder.Error}";
                _log.Error(error);
                symbolsBought = null;
                return false;
            }

            var isBought = PlaceTestOrders
                ? buyOrder.Data.Status == OrderStatus.New
                : buyOrder.Data.Status == OrderStatus.Filled;
            if (!isBought)
            {
                error = $"Failure placing buy order for {buyAction.Symbol}: {buyOrder.Data.Status}";
                _log.Error(error);
                symbolsBought = null;
                return false;
            }

            symbolsBought = PlaceTestOrders
                ? new Symbol
                {
                    SymbolName = buyAction.Symbol,
                    Price  = testPrice,
                    QuoteQuantity = buyAction.QuotedQuantity,
                    Quantity = buyAction.QuotedQuantity / testPrice,
                }
                : new Symbol
                {
                    SymbolName = buyAction.Symbol,
                    Price  = buyOrder.Data.Price,
                    QuoteQuantity = buyOrder.Data.QuoteQuantityFilled,
                    Quantity = buyOrder.Data.QuantityFilled
                };
            error = null;
            return true;
        }

        public bool TryConvert(SellAction sellAction, BuyAction buyAction, string referenceSymbol, CancellationToken cancellationToken, Func<DateTime> getNow, out TradeTransaction transaction, out string error)
        {
            var exchangeResult = _client.Spot.System.GetExchangeInfo();
            if (!exchangeResult.Success)
            {
                error = $"Failed to fetch exchange info: {exchangeResult.Error}";
                _log.Error(error);
                transaction = null;
                return false;
            }
            var exchangeInfo = exchangeResult.Data;

            decimal buyPrice = 0m, sellPrice = 0m;
            if (PlaceTestOrders)
            {
                if (!TryGetPrice(buyAction.Symbol, referenceSymbol, cancellationToken, out buyPrice, out error))
                {
                    _log.Error(error);
                    transaction = null;
                    return false;
                }
                if (!TryGetPrice(sellAction.Symbol, referenceSymbol, cancellationToken, out sellPrice, out error))
                {
                    _log.Error(error);
                    transaction = null;
                    return false;
                }
            }
            
            var orderResponseType = OrderResponseType.Full;
            var timeInForce = (TimeInForce?) null;// TimeInForce.FillOrKill;
            var orderType = OrderType.Market;

            if (!_validator.TryValidate(_client, sellAction, "Sell", referenceSymbol, exchangeInfo, cancellationToken, out sellAction, out error))
            {
                _log.Error(error);
                transaction = null;
                return false;
            }
            if(!_validator.TryValidate(_client, buyAction, "Buy", referenceSymbol, exchangeInfo, cancellationToken, out buyAction, out error))
            {
                _log.Error(error);
                transaction = null;
                return false;
            }
            
            var sellCoin = $"{sellAction.Symbol}{referenceSymbol}";

            // ReSharper disable ExpressionIsAlwaysNull
            var sellOrder = PlaceTestOrders
                ? _client.Spot.Order.PlaceTestOrder(sellCoin, OrderSide.Sell, orderType, null, sellAction.QuotedQuantity, sellAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken)
                : _client.Spot.Order.PlaceOrder(sellCoin, OrderSide.Sell, orderType, null, sellAction.QuotedQuantity, sellAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken);
            // ReSharper restore ExpressionIsAlwaysNull

            if (sellOrder.Error != null)
            {
                error = $"Failure placing sell order for {sellAction.Symbol}: {sellOrder.Error}";
                _log.Error(error);
                transaction = null;
                return false;
            }

            var isSold = PlaceTestOrders
                ? sellOrder.Data.Status == OrderStatus.New
                : sellOrder.Data.Status == OrderStatus.Filled;
            if (!isSold)
            {
                error = $"Failure placing sell order for {sellAction.Symbol}: {sellOrder.Data.Status}";
                _log.Error(error);
                transaction = null;
                return false;
            }
            
            var buyCoin = $"{buyAction.Symbol}{referenceSymbol}";

            // ReSharper disable ExpressionIsAlwaysNull
            var buyOrder = PlaceTestOrders
                ? _client.Spot.Order.PlaceTestOrder(buyCoin, OrderSide.Buy, orderType, null, buyAction.QuotedQuantity, buyAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken)
                : _client.Spot.Order.PlaceOrder(buyCoin, OrderSide.Buy, orderType, null, buyAction.QuotedQuantity, buyAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken);
            // ReSharper restore ExpressionIsAlwaysNull

            if (buyOrder.Error != null)
            {
                error = $"Failure placing buy order for {buyAction.Symbol}: {buyOrder.Error}";
                _log.Error(error);
                RollbackOrder(sellOrder.Data, cancellationToken);
                transaction = null;
                return false;
            }

            var isBought = PlaceTestOrders
                ? buyOrder.Data.Status == OrderStatus.New
                : buyOrder.Data.Status == OrderStatus.Filled;
            if (!isBought)
            {
                error = $"Failure placing buy order for {buyAction.Symbol}: {buyOrder.Data.Status}";
                _log.Error(error);
                RollbackOrder(sellOrder.Data, cancellationToken);
                transaction = null;
                return false;
            }

            var coinsSold = PlaceTestOrders
                ? new Symbol
                {
                    SymbolName = sellAction.Symbol,
                    Price = sellPrice,
                    QuoteQuantity = sellAction.QuotedQuantity,
                    Quantity = sellAction.QuotedQuantity / sellPrice
                }
                : new Symbol
                {
                    SymbolName = sellAction.Symbol,
                    Price = sellOrder.Data.Price,
                    QuoteQuantity = sellOrder.Data.QuoteQuantityFilled,
                    Quantity = sellOrder.Data.QuantityFilled
                };
            var coinsBought = PlaceTestOrders
                ? new Symbol
                {
                    SymbolName = buyAction.Symbol,
                    Price  = buyPrice,
                    QuoteQuantity = buyAction.QuotedQuantity,
                    Quantity = buyAction.QuotedQuantity / buyPrice,
                }
                :  new Symbol
                {
                    SymbolName = buyAction.Symbol,
                    Price  = buyOrder.Data.Price,
                    QuoteQuantity = buyOrder.Data.QuoteQuantityFilled,
                    Quantity = buyOrder.Data.QuantityFilled
                }; 
            transaction = new TradeTransaction
            {
                Sell = coinsSold,
                Buy = coinsBought,
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
                throw new InvalidOperationException(message);
            }
        }
    }
}