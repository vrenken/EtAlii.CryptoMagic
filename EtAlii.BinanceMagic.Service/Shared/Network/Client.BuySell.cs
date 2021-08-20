namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Binance.Net;
    using Binance.Net.Enums;
    using Binance.Net.Objects.Spot.SpotData;
    using Serilog;

    public partial class Client
    {
        private readonly ILogger _log = Log.ForContext<Client>();
        public async Task<(bool success, Symbol symbolsSold, string error)> TrySell(SellAction sellAction, string referenceSymbol, CancellationToken cancellationToken, Func<DateTime> getNow)
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
                ? await _client.Spot.Order.PlaceTestOrderAsync(sellSymbol, OrderSide.Sell, orderType, sellAction.Quantity, null, sellAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken)
                : await _client.Spot.Order.PlaceOrderAsync(sellSymbol, OrderSide.Sell, orderType, sellAction.Quantity, null, sellAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken);
            // ReSharper restore ExpressionIsAlwaysNull

            if (sellOrder.Error != null)
            {
                var error = $"Failure placing sell order for {sellAction.Symbol}: {sellOrder.Error}";
                _log.Error(error);
                return (false, null, error);
            }

            var isSold = PlaceTestOrders
                ? sellOrder.Data.Status == OrderStatus.New
                : sellOrder.Data.Status == OrderStatus.Filled;
            if (!isSold)
            {
                var error = $"Failure placing sell order for {sellAction.Symbol}: {sellOrder.Data.Status}";
                _log.Error(error);
                return (false, null, error);
            }

            var symbolsSold = new Symbol
            {
                SymbolName = sellAction.Symbol,
                Price = sellOrder.Data.Price,
                QuoteQuantity = sellOrder.Data.QuoteQuantityFilled,
                Quantity = sellOrder.Data.QuantityFilled
            };
            return (true, symbolsSold, null);
        }

        public async Task<(bool success, TradeTransaction transaction, string error)> TryBuyTransaction(BuyAction buyAction, string referenceSymbol, CancellationToken cancellationToken, Func<DateTime> getNow)
        {
            var (success, symbolsBought, error) = await TryBuySymbol(buyAction, referenceSymbol, cancellationToken, getNow);
            if (!success)
            {
                return (false, null, error);
            }
            var transaction = new TradeTransaction
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
            return (true, transaction, null);
        }

        public async Task<(bool success, Symbol symbolsBought, string error)> TryBuySymbol(BuyAction buyAction, string referenceSymbol, CancellationToken cancellationToken, Func<DateTime> getNow)
        {
            bool success;
            string error;
            var exchangeResult = await _client.Spot.System.GetExchangeInfoAsync(cancellationToken);
            if (!exchangeResult.Success)
            {
                error = $"Failed to fetch exchange info: {exchangeResult.Error}";
                _log.Error(error);
                return (false, null, error);
            }
            var exchangeInfo = exchangeResult.Data;

            decimal testPrice = 0;
            if (PlaceTestOrders)
            {
                (success, testPrice, error) = await TryGetPrice(buyAction.Symbol, referenceSymbol, cancellationToken);
                if (!success)
                {
                    _log.Error(error);
                    return (false, null, error);
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
                ? await _client.Spot.Order.PlaceTestOrderAsync(buySymbol, OrderSide.Buy, orderType, null, buyPrice, buyAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken)
                : await _client.Spot.Order.PlaceOrderAsync(buySymbol, OrderSide.Buy, orderType, null, buyPrice, buyAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken);
            // ReSharper restore ExpressionIsAlwaysNull

            if (buyOrder.Error != null)
            {
                error = $"Failure placing buy order for {buyAction.Symbol}: {buyOrder.Error}";
                _log.Error(error);
                return (false, null, error);
            }

            var isBought = PlaceTestOrders
                ? buyOrder.Data.Status == OrderStatus.New
                : buyOrder.Data.Status == OrderStatus.Filled;
            if (!isBought)
            {
                error = $"Failure placing buy order for {buyAction.Symbol}: {buyOrder.Data.Status}";
                _log.Error(error);
                return (false, null, error);
            }

            var symbolsBought = PlaceTestOrders
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
            return (true, symbolsBought, null);
        }

        public async Task<(bool success, TradeTransaction transaction, string error)> TryConvert(SellAction sellAction, BuyAction buyAction, string referenceSymbol, CancellationToken cancellationToken, Func<DateTime> getNow)
        {
            bool success;
            string error;
            decimal buyPrice = 0m;
            decimal sellPrice = 0m;
            var exchangeResult = await _client.Spot.System.GetExchangeInfoAsync(cancellationToken);
            if (!exchangeResult.Success)
            {
                error = $"Failed to fetch exchange info: {exchangeResult.Error}";
                _log.Error(error);
                return (false, null, error);
            }
            var exchangeInfo = exchangeResult.Data;

            if (PlaceTestOrders)
            {
                (success, buyPrice, error) = await TryGetPrice(buyAction.Symbol, referenceSymbol, cancellationToken);
                if (!success)
                {
                    _log.Error(error);
                    return (false, null, error);
                }

                (success, sellPrice, error) = await TryGetPrice(sellAction.Symbol, referenceSymbol, cancellationToken);
                if (!success)
                {
                    _log.Error(error);
                    return (false, null, error);
                }
            }
            
            var orderResponseType = OrderResponseType.Full;
            var timeInForce = (TimeInForce?) null;// TimeInForce.FillOrKill;
            var orderType = OrderType.Market;

            (success, _, error) = await _validator.TryValidate(_client, sellAction, "Sell", referenceSymbol, exchangeInfo, cancellationToken); 
            if (!success)
            {
                _log.Error(error);
                return (false, null, error);
            }
            (success, _, error) = await _validator.TryValidate(_client, buyAction, "Buy", referenceSymbol, exchangeInfo, cancellationToken);
            if(!success)
            {
                _log.Error(error);
                return (false, null, error);
            }
            
            var sellCoin = $"{sellAction.Symbol}{referenceSymbol}";

            // ReSharper disable ExpressionIsAlwaysNull
            var sellOrder = PlaceTestOrders
                ? await _client.Spot.Order.PlaceTestOrderAsync(sellCoin, OrderSide.Sell, orderType, null, sellAction.QuotedQuantity, sellAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken)
                : await _client.Spot.Order.PlaceOrderAsync(sellCoin, OrderSide.Sell, orderType, null, sellAction.QuotedQuantity, sellAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken);
            // ReSharper restore ExpressionIsAlwaysNull

            if (sellOrder.Error != null)
            {
                error = $"Failure placing sell order for {sellAction.Symbol}: {sellOrder.Error}";
                _log.Error(error);
                return (false, null, error);
            }

            var isSold = PlaceTestOrders
                ? sellOrder.Data.Status == OrderStatus.New
                : sellOrder.Data.Status == OrderStatus.Filled;
            if (!isSold)
            {
                error = $"Failure placing sell order for {sellAction.Symbol}: {sellOrder.Data.Status}";
                _log.Error(error);
                return (false, null, error);
            }
            
            var buyCoin = $"{buyAction.Symbol}{referenceSymbol}";

            // ReSharper disable ExpressionIsAlwaysNull
            var buyOrder = PlaceTestOrders
                ? await _client.Spot.Order.PlaceTestOrderAsync(buyCoin, OrderSide.Buy, orderType, null, buyAction.QuotedQuantity, buyAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken)
                : await _client.Spot.Order.PlaceOrderAsync(buyCoin, OrderSide.Buy, orderType, null, buyAction.QuotedQuantity, buyAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, cancellationToken);
            // ReSharper restore ExpressionIsAlwaysNull

            if (buyOrder.Error != null)
            {
                error = $"Failure placing buy order for {buyAction.Symbol}: {buyOrder.Error}";
                _log.Error(error);
                await RollbackOrder(sellOrder.Data, cancellationToken);
                return (false, null, error);
            }

            var isBought = PlaceTestOrders
                ? buyOrder.Data.Status == OrderStatus.New
                : buyOrder.Data.Status == OrderStatus.Filled;
            if (!isBought)
            {
                error = $"Failure placing buy order for {buyAction.Symbol}: {buyOrder.Data.Status}";
                _log.Error(error);
                await RollbackOrder(sellOrder.Data, cancellationToken);
                return (false, null, error);
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
            var transaction = new TradeTransaction
            {
                Sell = coinsSold,
                Buy = coinsBought,
                Moment = getNow(),
                Profit = sellOrder.Data.QuoteQuantityFilled - buyOrder.Data.QuoteQuantityFilled 
            };
            return (true, transaction, error);
        }
        
        private async Task RollbackOrder(BinancePlacedOrder order, CancellationToken cancellationToken)
        {
            var cancelOrder = await _client.Spot.Order.CancelOrderAsync(order.Symbol, order.OrderId, order.ClientOrderId, ct: cancellationToken);
            if (cancelOrder.Error != null)
            {
                var message = $"Failure cancelling order for {order.ClientOrderId}: {cancelOrder.Error}";
                throw new InvalidOperationException(message);
            }
        }
    }
}