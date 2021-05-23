namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Threading;

    public partial class Sequence : ISequence
    {
        private readonly ICircularAlgorithm _circularAlgorithm;
        private readonly ITradeDetailsUpdater _detailsUpdater;
        private readonly IClient _client;
        private readonly ITimeManager _timeManager;

        public IAlgorithmContext<CircularTransaction, CircularTrading> Status => _context;
        private readonly IAlgorithmContext<CircularTransaction, CircularTrading> _context;
        private readonly Action _initialize;

        public Sequence(
            IClient client,
            ITimeManager timeManager,
            IAlgorithmContext<CircularTransaction, CircularTrading> context, Action initialize = null)
        {
            _client = client;
            _timeManager = timeManager;
            _context = context;
            _initialize = initialize;

            _detailsUpdater = new TradeDetailsUpdater(_context);
            _circularAlgorithm = new CircularAlgorithm(client, _context);
        }

        public void Initialize(CancellationToken cancellationToken) => _initialize?.Invoke();

        private CircularTransaction GetFirstTransaction()
        {
            var data = new DataContext();
            
            var transaction = data.FindPreviousTransaction(_context.Trading);
            return transaction ?? new CircularTransaction
            {
                Trading = _context.Trading,
            };
        }
        public void Run(CancellationToken cancellationToken, out bool keepRunning)
        {
            var transaction = _context.CurrentTransaction ?? GetFirstTransaction();

            _detailsUpdater.UpdateTargetDetails(transaction);
            transaction.LastCheck = _timeManager.GetNow();
            _context.Update(transaction);
            
            var targetAchieved = false;
            var shouldDelay = false;

            while (!targetAchieved)
            {
                if (shouldDelay)
                {
                    transaction.NextCheck = _timeManager.GetNow() + _context.Trading.SampleInterval;

                    if (_timeManager.ShouldStop())
                    {
                        transaction.Result = "Back-test completed";
                        _context.Trading.End = DateTime.Now;
                        _context.Update(_context.Trading, transaction);
                        keepRunning = false;
                        return;
                    }

                    _context.Update(_context.Trading, transaction);

                    _timeManager.Wait(_context.Trading.SampleInterval, cancellationToken);
                }
                transaction.NextCheck = null;
                transaction.Result = "Fetching current situation...";
                transaction.LastCheck = _timeManager.GetNow();
                _context.Update(transaction);

                if (!TryGetSituation(transaction, cancellationToken, out var situation, out var error))
                {
                    transaction.Result = error;
                    _context.Update(transaction);
                    shouldDelay = true;
                    continue;
                }
                
                transaction.Result = "Fetching exchange info...";
                transaction.LastCheck = _timeManager.GetNow();
                _context.Update(transaction);

                if (!_client.TryGetExchangeInfo(cancellationToken, out var exchangeInfo, out error))
                {
                    shouldDelay = true;
                    transaction.Result = error;
                    _context.Update(transaction);
                    continue;
                }
                situation = situation with { ExchangeInfo = exchangeInfo };
                
                if (situation.IsInitialCycle)
                {
                    targetAchieved = HandleInitialCycle(cancellationToken, situation, transaction);
                    shouldDelay = !targetAchieved;
                    continue;
                }
                if(TryHandleNormalCycle(cancellationToken, situation, out targetAchieved))
                {
                    shouldDelay = !targetAchieved;
                    continue;
                }

                transaction.Result = "No feasible transaction";
                transaction.LastCheck = _timeManager.GetNow();
                _context.Update(transaction);
                shouldDelay = true;
            }

            keepRunning = true;
        }

        private bool TryGetSituation(CircularTransaction transaction, CancellationToken cancellationToken, out Situation situation, out string error)
        {
            if (!_client.TryGetPrice(transaction.SellSymbol, _context.Trading.ReferenceSymbol, cancellationToken, out var sourcePrice, out error))
            {
                situation = null;
                return false;
            }

            if (!_client.TryGetPrice(transaction.BuySymbol, _context.Trading.ReferenceSymbol, cancellationToken, out var targetPrice, out error))
            {
                situation = null;
                return false;
            }

            if (!_client.TryGetTradeFees(transaction.SellSymbol, _context.Trading.ReferenceSymbol, cancellationToken, out var sourceMakerFee, out var _, out error))
            {
                situation = null;
                return false;
            }
            
            if (!_client.TryGetTradeFees(transaction.BuySymbol, _context.Trading.ReferenceSymbol, cancellationToken, out var _, out var destinationTakerFee, out error))
            {
                situation = null;
                return false;
            }

            if (!_client.TryGetTrend(transaction.SellSymbol, _context.Trading.ReferenceSymbol, _context.Trading.RsiPeriod, cancellationToken, out var sellTrend, out error))
            {
                situation = null;
                return false;
            }
            if (!_client.TryGetTrend(transaction.BuySymbol, _context.Trading.ReferenceSymbol, _context.Trading.RsiPeriod, cancellationToken, out var buyTrend, out error))
            {
                situation = null;
                return false;
            }

            using var data = new DataContext();
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

            situation = new Situation
            {
                Source = sourceDelta,
                SellFee = sourceMakerFee,
                SellTrend = sellTrend,
                Destination = targetDelta,
                BuyFee = destinationTakerFee,
                BuyTrend = buyTrend,
                IsInitialCycle = data.IsInitialCycle(_context.Trading) 
            };

            error = null;
            return true;
        }
        
        
        private void SaveAndReplaceTransaction(CircularTransaction transaction, TradeTransaction tradeTransaction)
        {
            transaction.SellSymbol = tradeTransaction.Sell.SymbolName;
            transaction.SellPrice = tradeTransaction.Sell.QuoteQuantity;
            transaction.SellQuantity = tradeTransaction.Sell.Quantity;

            transaction.BuySymbol = tradeTransaction.Buy.SymbolName;
            transaction.BuyPrice = tradeTransaction.Buy.QuoteQuantity;
            transaction.BuyQuantity = tradeTransaction.Buy.Quantity;

            transaction.Result = $"Transaction done";
            transaction.LastCheck = _timeManager.GetNow();
            transaction.LastSuccess = _timeManager.GetNow(); 
            
            transaction.Profit = transaction.SellPrice - transaction.BuyPrice;
            
                    
            using var data = new DataContext();
            _context.Trading.TotalProfit = transaction.Profit + data.GetTotalProfits(_context.Trading);

            transaction.Completed = true;
            transaction = transaction.ShallowClone();
            _context.Update(_context.Trading, transaction);
        }
    }
}