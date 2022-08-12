namespace EtAlii.CryptoMagic
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Binance.Net;
    using Binance.Net.Enums;
    using Binance.Net.Objects.Models.Spot;
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

            string error;
            decimal testPrice = 0;
            if (PlaceTestOrders)
            {
                bool success;
                (success, testPrice, error) = await TryGetPrice(sellAction.Symbol, referenceSymbol, cancellationToken);
                if (!success)
                {
                    return (false, null, error);
                }
            }

            var orderResponseType = OrderResponseType.Full;
            var timeInForce = (TimeInForce?) null;// TimeInForce.FillOrKill;
            var orderType = SpotOrderType.Market;

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
                ? await _client.SpotApi.Trading.PlaceTestOrderAsync(sellSymbol, OrderSide.Sell, orderType, sellAction.Quantity, null, sellAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, null, cancellationToken)
                : await _client.SpotApi.Trading.PlaceOrderAsync(sellSymbol, OrderSide.Sell, orderType, sellAction.Quantity, null, sellAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, null, cancellationToken);
            // ReSharper restore ExpressionIsAlwaysNull

            if (sellOrder.Error != null)
            {
                _log.Error("Failure placing sell order for {Symbol}: {Error}", sellAction.Symbol, sellOrder.Error);
                return (false, null, $"Failure placing sell order for {sellAction.Symbol}: {sellOrder.Error}");
            }

            var isSold = PlaceTestOrders
                ? sellOrder.Data.Status == OrderStatus.New
                : sellOrder.Data.Status == OrderStatus.Filled;
            if (!isSold)
            {
                _log.Error("Failure placing sell order for {Symbol}: {Status}", sellAction.Symbol, sellOrder.Data.Status);
                return (false, null, $"Failure placing sell order for {sellAction.Symbol}: {sellOrder.Data.Status}");
            }

            var symbolsSold = PlaceTestOrders
                ? new Symbol
                {
                    SymbolName = sellAction.Symbol,
                    Price  = testPrice,
                    QuoteQuantity = sellAction.Quantity * testPrice,
                    Quantity = sellAction.Quantity,
                }
                : new Symbol
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
            string error;
            var exchangeResult = await _client.SpotApi.ExchangeData.GetExchangeInfoAsync(cancellationToken);
            if (!exchangeResult.Success)
            {
                _log.Error("Failed to fetch exchange info: {Error}", exchangeResult.Error);
                return (false, null, $"Failed to fetch exchange info: {exchangeResult.Error}");
            }
            var exchangeInfo = exchangeResult.Data;

            decimal testPrice = 0;
            if (PlaceTestOrders)
            {
                bool success;
                (success, testPrice, error) = await TryGetPrice(buyAction.Symbol, referenceSymbol, cancellationToken);
                if (!success)
                {
                    return (false, null, error);
                }
            }

            var orderResponseType = OrderResponseType.Full;
            var timeInForce = (TimeInForce?) null;// TimeInForce.FillOrKill;
            var orderType = SpotOrderType.Market;

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
                ? await _client.SpotApi.Trading.PlaceTestOrderAsync(buySymbol, OrderSide.Buy, orderType, null, buyPrice, buyAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, null, cancellationToken)
                : await _client.SpotApi.Trading.PlaceOrderAsync(buySymbol, OrderSide.Buy, orderType, null, buyPrice, buyAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, null, cancellationToken);
            // ReSharper restore ExpressionIsAlwaysNull

            if (buyOrder.Error != null)
            {
                _log.Error("Failure placing buy order for {Symbol}: {Error}", buyAction.Symbol, buyOrder.Error);
                return (false, null, $"Failure placing buy order for {buyAction.Symbol}: {buyOrder.Error}");
            }

            var isBought = PlaceTestOrders
                ? buyOrder.Data.Status == OrderStatus.New
                : buyOrder.Data.Status == OrderStatus.Filled;
            if (!isBought)
            {
                _log.Error("Failure placing buy order for {Symbol}: {Status}", buyAction.Symbol, buyOrder.Data.Status);
                return (false, null, $"Failure placing buy order for {buyAction.Symbol}: {buyOrder.Data.Status}");
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
            var exchangeResult = await _client.SpotApi.ExchangeData.GetExchangeInfoAsync(cancellationToken);
            if (!exchangeResult.Success)
            {
                _log.Error("Failed to fetch exchange info: {Error}", exchangeResult.Error);
                return (false, null, $"Failed to fetch exchange info: {exchangeResult.Error}");
            }
            var exchangeInfo = exchangeResult.Data;

            if (PlaceTestOrders)
            {
                (success, buyPrice, error) = await TryGetPrice(buyAction.Symbol, referenceSymbol, cancellationToken);
                if (!success)
                {
                    return (false, null, error);
                }

                (success, sellPrice, error) = await TryGetPrice(sellAction.Symbol, referenceSymbol, cancellationToken);
                if (!success)
                {
                    return (false, null, error);
                }
            }
            
            var orderResponseType = OrderResponseType.Full;
            var timeInForce = (TimeInForce?) null;// TimeInForce.FillOrKill;
            var orderType = SpotOrderType.Market;

            (success, _, error) = await _validator.TryValidate(_client, sellAction, "Sell", referenceSymbol, exchangeInfo, cancellationToken); 
            if (!success)
            {
                return (false, null, error);
            }
            (success, _, error) = await _validator.TryValidate(_client, buyAction, "Buy", referenceSymbol, exchangeInfo, cancellationToken);
            if(!success)
            {
                return (false, null, error);
            }
            
            var sellCoin = $"{sellAction.Symbol}{referenceSymbol}";

            // ReSharper disable ExpressionIsAlwaysNull
            var sellOrder = PlaceTestOrders
                ? await _client.SpotApi.Trading.PlaceTestOrderAsync(sellCoin, OrderSide.Sell, orderType, null, sellAction.QuotedQuantity, sellAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, null, cancellationToken)
                : await _client.SpotApi.Trading.PlaceOrderAsync(sellCoin, OrderSide.Sell, orderType, null, sellAction.QuotedQuantity, sellAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, null, cancellationToken);
            // ReSharper restore ExpressionIsAlwaysNull

            if (sellOrder.Error != null)
            {
                _log.Error("Failure placing sell order for {Symbol}: {Error}", sellAction.Symbol, sellOrder.Error);
                return (false, null, $"Failure placing sell order for {sellAction.Symbol}: {sellOrder.Error}");
            }

            var isSold = PlaceTestOrders
                ? sellOrder.Data.Status == OrderStatus.New
                : sellOrder.Data.Status == OrderStatus.Filled;
            if (!isSold)
            {
                _log.Error("Failure placing sell order for {Symbol}: {Status}", sellAction.Symbol, sellOrder.Data.Status);
                return (false, null, $"Failure placing sell order for {sellAction.Symbol}: {sellOrder.Data.Status}");
            }
            
            var buyCoin = $"{buyAction.Symbol}{referenceSymbol}";

            // ReSharper disable ExpressionIsAlwaysNull
            var buyOrder = PlaceTestOrders
                ? await _client.SpotApi.Trading.PlaceTestOrderAsync(buyCoin, OrderSide.Buy, orderType, null, buyAction.QuotedQuantity, buyAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, null, cancellationToken)
                : await _client.SpotApi.Trading.PlaceOrderAsync(buyCoin, OrderSide.Buy, orderType, null, buyAction.QuotedQuantity, buyAction.TransactionId, null, timeInForce, null, null, orderResponseType, null, null, cancellationToken);
            // ReSharper restore ExpressionIsAlwaysNull

            if (buyOrder.Error != null)
            {
                _log.Error("Failure placing buy order for {Symbol}: {Error}", buyAction.Symbol, buyOrder.Error);
                await RollbackOrder(sellOrder.Data, cancellationToken);
                return (false, null, $"Failure placing buy order for {buyAction.Symbol}: {buyOrder.Error}");
            }

            var isBought = PlaceTestOrders
                ? buyOrder.Data.Status == OrderStatus.New
                : buyOrder.Data.Status == OrderStatus.Filled;
            if (!isBought)
            {
                _log.Error("Failure placing buy order for {Symbol}: {Status}", buyAction.Symbol, buyOrder.Data.Status);
                await RollbackOrder(sellOrder.Data, cancellationToken);
                return (false, null, $"Failure placing buy order for {buyAction.Symbol}: {buyOrder.Data.Status}");
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
            var cancelOrder = await _client.SpotApi.Trading.CancelOrderAsync(order.Symbol, order.Id, order.ClientOrderId, ct: cancellationToken);
            if (cancelOrder.Error != null)
            {
                var message = $"Failure cancelling order for {order.ClientOrderId}: {cancelOrder.Error}";
                throw new InvalidOperationException(message);
            }
        }
    }
}