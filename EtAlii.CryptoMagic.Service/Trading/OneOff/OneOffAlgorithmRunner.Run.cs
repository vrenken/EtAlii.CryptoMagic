namespace EtAlii.CryptoMagic.Service
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public partial class OneOffAlgorithmRunner 
    {
        private async Task RunStep()
        {
            bool success;
            decimal price;

            (success, price, _) = await _client.TryGetPrice(_trading.Symbol, _trading.ReferenceSymbol, CancellationToken.None);

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
                    var sellAction = new SellAction
                    {
                        Symbol = _trading.Symbol,
                        Price = _trading.CurrentPrice,
                        Quantity = _trading.PurchaseSymbolQuantity,
                        QuotedQuantity = 0m, // not used.
                    };
                    Symbol symbol;
                    (success, symbol, _) = await _client.TrySell(sellAction, _trading.ReferenceSymbol, CancellationToken.None, () => DateTime.Now);
                    if (success)
                    {
                        _trading.FinalQuoteQuantity = symbol.QuoteQuantity;
                        _trading.End = DateTime.Now;
                        _trading.IsSuccess = true;
                    }
                }
                await using var data = new DataContext();

                data.Entry(_trading).State = EntityState.Modified;

                await data.SaveChangesAsync();
            }

            Changed?.Invoke(this);

            if (_trading.IsSuccess)
            {
                await Stop();
            }
        }
    }
}