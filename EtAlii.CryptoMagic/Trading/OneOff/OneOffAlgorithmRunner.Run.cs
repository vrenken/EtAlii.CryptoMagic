namespace EtAlii.CryptoMagic
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Serilog;

    public partial class OneOffAlgorithmRunner
    {
        private readonly ILogger _log = Serilog.Log.ForContext<OneOffAlgorithmRunner>();
        private async Task RunStep()
        {
            bool success;
            decimal price;
            string error;

            (success, price, error) = await _client.TryGetPrice(_trading.Symbol, _trading.ReferenceSymbol, CancellationToken.None);

            if (success)
            {
                _trading.CurrentPrice = price;
                
                var purchaseValue = _trading.PurchaseQuoteQuantity; 
                var currentValue = _trading.PurchaseSymbolQuantity * _trading.CurrentPrice;
                var onePercent = purchaseValue / 100m;
                _trading.CurrentPercentageIncrease = currentValue / onePercent - 100;
                _trading.FinalQuoteQuantity = currentValue;

                if (_trading.CurrentPercentageIncrease > _trading.TargetPercentageIncrease)
                {
                    _log.Information("Trade {TradeName} is worth it: {ActualValue} over {TargetValue}", _trading.Name, _trading.CurrentPercentageIncrease, _trading.TargetPercentageIncrease);                    

                    var sellAction = new SellAction
                    {
                        Symbol = _trading.Symbol,
                        Price = _trading.CurrentPrice,
                        Quantity = _trading.PurchaseSymbolQuantity,
                        QuotedQuantity = 0m, // not used.
                    };
                    Symbol symbol;
                    (success, symbol, error) = await _client.TrySell(sellAction, _trading.ReferenceSymbol, CancellationToken.None, () => DateTime.Now);
                    if (success)
                    {
                        _trading.FinalQuoteQuantity = symbol.QuoteQuantity;
                        _trading.End = DateTime.Now;
                        _trading.IsSuccess = true;
                    }
                    else
                    {
                        _log.Error("Unable to sell for trade {TradeName}: {ErrorMessage}", _trading.Name, error);                    
                    }
                }
                else
                {
                    _log.Information("Trade {TradeName} still not worth it: {ActualValue} instead of {TargetValue}", _trading.Name, _trading.CurrentPercentageIncrease, _trading.TargetPercentageIncrease);                    
                }

                await using var data = new DataContext();

                data.Entry(_trading).State = EntityState.Modified;

                await data.SaveChangesAsync();
            }
            else
            {
                _log.Error("For trade {TradeName} the price cannot be retrieved: {ErrorMessage}", _trading.Name, error);                    
            }

            Changed?.Invoke(this);

            if (_trading.IsSuccess)
            {
                await Stop();
            }
        }
    }
}