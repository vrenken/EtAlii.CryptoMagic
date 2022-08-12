namespace EtAlii.CryptoMagic
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Binance.Net.Objects.Models.Spot;
    using Microsoft.EntityFrameworkCore;

    public partial class Sequence : ISequence
    {
        private readonly ICircularAlgorithm _circularAlgorithm;
        private readonly ITargetTransactionFinder _targetTransactionFinder;
        private readonly IClient _client;
        private readonly ITimeManager _timeManager;

        public IAlgorithmContext<CircularTransaction, CircularTrading> Status => _context;
        private readonly IAlgorithmContext<CircularTransaction, CircularTrading> _context;
        private readonly Func<Task> _initialize;

        public Sequence(
            IClient client,
            ITimeManager timeManager,
            IAlgorithmContext<CircularTransaction, CircularTrading> context, Func<Task> initialize = null)
        {
            _client = client;
            _timeManager = timeManager;
            _context = context;
            _initialize = initialize;

            _targetTransactionFinder = new TargetTransactionFinder(_context);
            _circularAlgorithm = new CircularAlgorithm(client, _context);
        }

        public Task Initialize(CancellationToken cancellationToken) => _initialize?.Invoke();

        public async Task<bool> Run(CancellationToken cancellationToken)
        {
            var transaction = _targetTransactionFinder.Find();

            transaction.LastCheck = _timeManager.GetNow();
            _context.Update(transaction);
            
            var targetAchieved = false;
            var shouldDelay = false;

            while (!targetAchieved)
            {
                if (shouldDelay)
                {
                    transaction.NextCheck = _timeManager.GetNow() + _context.Trading.SampleInterval;
                    transaction.IsChanging = false;
                    
                    if (_timeManager.ShouldStop())
                    {
                        transaction.Result = "Back-test completed";
                        transaction.NextCheck = null;
                        _context.Trading.End = DateTime.Now;
                        _context.Update(transaction);
                        return false;
                    }

                    _context.Update(transaction);

                    _timeManager.Wait(_context.Trading.SampleInterval, cancellationToken);
                }

                transaction.IsChanging = true;
                transaction.NextCheck = null;
                transaction.Result = "Fetching current situation...";
                transaction.LastCheck = _timeManager.GetNow();
                _context.Update(transaction);

                var (success, situation, error) = await TryGetSituation(transaction, cancellationToken);
                if (!success)
                {
                    transaction.Result = error;
                    _context.Update(transaction);
                    shouldDelay = true;
                    continue;
                }
                
                transaction.Result = "Fetching exchange info...";
                transaction.LastCheck = _timeManager.GetNow();
                _context.Update(transaction);

                BinanceExchangeInfo exchangeInfo;
                (success, exchangeInfo, error) = await _client.TryGetExchangeInfo(cancellationToken);
                if (!success)
                {
                    shouldDelay = true;
                    transaction.Result = error;
                    _context.Update(transaction);
                    continue;
                }
                
                situation = situation with { ExchangeInfo = exchangeInfo };
                if (situation.IsInitialCycle)
                {
                    targetAchieved = await HandleInitialCycle(cancellationToken, situation, transaction);
                    shouldDelay = !targetAchieved;
                    continue;
                }

                (success, targetAchieved) = await TryHandleNormalCycle(cancellationToken, situation);
                if(success)
                {
                    shouldDelay = !targetAchieved;
                    continue;
                }

                transaction.Result = "No feasible transaction";
                transaction.LastCheck = _timeManager.GetNow();
                _context.Update(transaction);
                shouldDelay = true;
            }

            return true;
        }

        private async Task<(bool success, Situation situation, string error)> TryGetSituation(CircularTransaction transaction, CancellationToken cancellationToken)
        {
            bool success;
            string error;
            decimal sourcePrice;
            decimal targetPrice;
            (success, sourcePrice, error) = await _client.TryGetPrice(transaction.SellSymbol, _context.Trading.ReferenceSymbol, cancellationToken);
            if (!success)
            {
                return (false, null, error);
            }

            (success, targetPrice, error) = await _client.TryGetPrice(transaction.BuySymbol, _context.Trading.ReferenceSymbol, cancellationToken);
            if (!success)
            {
                return (false, null, error);
            }
            
            decimal sourceMakerFee;
            (success, sourceMakerFee, _, error) = await _client.TryGetTradeFees(transaction.SellSymbol, _context.Trading.ReferenceSymbol, cancellationToken);
            if (!success)
            {
                return (false, null, error);
            }

            decimal destinationTakerFee;
            (success, _, destinationTakerFee, error) = await _client.TryGetTradeFees(transaction.BuySymbol, _context.Trading.ReferenceSymbol, cancellationToken);
            if (!success)
            {
                return (false, null, error);
            }

            decimal sellTrend;
            (success, sellTrend, error) = await _client.TryGetTrend(transaction.SellSymbol, _context.Trading.ReferenceSymbol, _context.Trading.RsiPeriod, cancellationToken); 
            if (!success)
            {
                return (false, null, error);
            }

            decimal buyTrend;
            (success, buyTrend, error) = await _client.TryGetTrend(transaction.BuySymbol, _context.Trading.ReferenceSymbol, _context.Trading.RsiPeriod, cancellationToken);
            if (!success)
            {
                return (false, null, error);
            }

            await using var data = new DataContext();
            var snapshot = new Snapshot
            {
                Moment = DateTime.Now,
                Trading = _context.Trading,
                FirstSymbolMarketPrice = transaction.SellSymbol == _context.Trading.FirstSymbol ? sourcePrice : targetPrice,
                SecondSymbolMarketPrice = transaction.SellSymbol == _context.Trading.FirstSymbol ? targetPrice : sourcePrice
            };

            data.Entry(snapshot).State = EntityState.Added;
            data.Entry(_context.Trading).State = EntityState.Unchanged;
            await data.SaveChangesAsync(cancellationToken);

            var lastSourcePurchase = data.FindLastPurchase(transaction.SellSymbol, _context.Trading, transaction);
            var sourceDelta = new Delta
            {
                Symbol = transaction.SellSymbol,
                PastPrice = lastSourcePurchase?.BuyPrice ?? sourcePrice,
                PastQuantity = lastSourcePurchase?.BuyQuantity ?? 0,
                PresentPrice = sourcePrice,
            };

            var lastTargetSell = data.FindLastSell(transaction.BuySymbol, _context.Trading, transaction);
            var targetDelta = new Delta
            {
                Symbol = transaction.BuySymbol,
                PastPrice = lastTargetSell?.SellPrice ?? targetPrice,
                PastQuantity = lastTargetSell?.SellQuantity ?? 0,
                PresentPrice = targetPrice
            };

            var situation = new Situation
            {
                Source = sourceDelta,
                SellFee = sourceMakerFee,
                SellTrend = sellTrend,
                Destination = targetDelta,
                BuyFee = destinationTakerFee,
                BuyTrend = buyTrend,
                IsInitialCycle = data.IsInitialCycle(_context.Trading) 
            };

            return (true, situation, null);
        }
        
        
        private void SaveAndReplaceTransaction(CircularTransaction transaction, TradeTransaction tradeTransaction, bool isInitialTransaction)
        {
            transaction.SellPrice = tradeTransaction.Sell.QuoteQuantity;
            transaction.SellQuantity = tradeTransaction.Sell.Quantity;

            transaction.BuyPrice = tradeTransaction.Buy.QuoteQuantity;
            transaction.BuyQuantity = tradeTransaction.Buy.Quantity;

            transaction.Result = $"Transaction done";
            transaction.LastCheck = _timeManager.GetNow();
            transaction.LastSuccess = _timeManager.GetNow();

            using var data = new DataContext();
            if (!isInitialTransaction)
            {
                transaction.Profit = transaction.SellPrice - transaction.BuyPrice;
                _context.Trading.TotalProfit = transaction.Profit + data.GetTotalProfits(_context.Trading);
            }
            else
            {
                transaction.Profit = 0;
                _context.Trading.TotalProfit = 0;
            }

            transaction.IsChanging = false;
            transaction.Completed = true;
            transaction = transaction.ShallowClone();
            _context.Update(transaction);
        }
    }
}