namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public partial class OneOffAlgorithmRunner 
    {
        public async Task Stop()
        {
            if (_timer != null)
            {
                await _timer.DisposeAsync();
            }
            Changed?.Invoke(this);
        }

        public void Cancel()
        {
            var sellAction = new SellAction
            {
                Symbol = _trading.Symbol,
                Price = _trading.CurrentPrice,
                Quantity = _trading.PurchaseSymbolQuantity,
                QuotedQuantity = 0m, // not used.
            };

            var task = Task.Run(async () =>
            {
                var (success, symbol, _) = await _client.TrySell(sellAction, _trading.ReferenceSymbol, CancellationToken.None, () => DateTime.Now);
                if (success)
                {
                    var task = Stop();
                    task.Wait();

                    await using var data = new DataContext();

                    _trading.FinalQuoteQuantity = symbol.QuoteQuantity;
                    _trading.IsCancelled = true;
                    data.OneOffTradings.Attach(Context.Trading).State = EntityState.Modified;
                    await data.SaveChangesAsync();
                }
            });

            task.Wait();
            
            Changed?.Invoke(this);
        }
    }
}