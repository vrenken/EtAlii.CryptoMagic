namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Threading;
    using Microsoft.EntityFrameworkCore;

    public partial class Sequence : ISequence
    {
        private readonly ICircularAlgorithm _circularAlgorithm;
        private readonly ITradeDetailsUpdater _detailsUpdater;
        private readonly IClient _client;
        private readonly ITimeManager _timeManager;

        public IAlgorithmContext<CircularTradeSnapshot, CircularTrading> Status => _context;
        private readonly IAlgorithmContext<CircularTradeSnapshot, CircularTrading> _context;
        private readonly Action _initialize;

        public Sequence(
            IClient client,
            ITimeManager timeManager, 
            IAlgorithmContext<CircularTradeSnapshot, CircularTrading> context, Action initialize = null)
        {
            _client = client;
            _timeManager = timeManager;
            _context = context;
            _initialize = initialize;

            _detailsUpdater = new TradeDetailsUpdater(_context);
            _circularAlgorithm = new CircularAlgorithm(client, _context);
        }

        public void Initialize(CancellationToken cancellationToken) => _initialize?.Invoke();
        
        public void Run(CancellationToken cancellationToken, out bool keepRunning)
        {
            var snapshot = _context.Snapshot;

            _detailsUpdater.UpdateTargetDetails(snapshot);
            snapshot.LastCheck = _timeManager.GetNow();
            _context.RaiseChanged();
            
            var targetAchieved = false;
            var shouldDelay = false;

            while (!targetAchieved)
            {
                if (shouldDelay)
                {
                    snapshot.NextCheck = _timeManager.GetNow() + _context.Trading.SampleInterval;

                    var isTransition = snapshot.NextCheck.Hour != snapshot.LastCheck.Hour;
                    _context.RaiseChanged(isTransition ? AlgorithmChange.Important : AlgorithmChange.Normal);

                    if (_timeManager.ShouldStop())
                    {
                        snapshot.Result = "Back-test completed";
                        _context.RaiseChanged(AlgorithmChange.Important);
                        keepRunning = false;
                        return;
                    }
                    
                    _timeManager.Wait(_context.Trading.SampleInterval, cancellationToken);
                }
                snapshot.NextCheck = DateTime.MinValue;
                snapshot.Result = "Fetching current situation...";
                snapshot.LastCheck = _timeManager.GetNow();
                _context.RaiseChanged();

                if (!TryGetSituation(snapshot, cancellationToken, out var situation, out var error))
                {
                    snapshot.Result = error;
                    _context.RaiseChanged();
                    shouldDelay = true;
                    continue;
                }
                
                snapshot.Result = "Fetching exchange info...";
                snapshot.LastCheck = _timeManager.GetNow();
                _context.RaiseChanged();

                if (!_client.TryGetExchangeInfo(cancellationToken, out var exchangeInfo, out error))
                {
                    shouldDelay = true;
                    snapshot.Result = error;
                    _context.RaiseChanged();
                    continue;
                }
                situation = situation with { ExchangeInfo = exchangeInfo };
                
                if (situation.IsInitialCycle)
                {
                    targetAchieved = HandleInitialCycle(cancellationToken, situation);
                    shouldDelay = !targetAchieved;
                    continue;
                }
                if(TryHandleNormalCycle(cancellationToken, situation, out targetAchieved))
                {
                    shouldDelay = !targetAchieved;
                    continue;
                }

                snapshot.Result = "No feasible transaction";
                snapshot.LastCheck = _timeManager.GetNow();
                _context.RaiseChanged();
                shouldDelay = true;
            }

            keepRunning = true;
        }

        private bool TryGetSituation(CircularTradeSnapshot snapshot, CancellationToken cancellationToken, out Situation situation, out string error)
        {
            if (!_client.TryGetPrice(snapshot.SellSymbol, _context.Trading.ReferenceSymbol, cancellationToken, out var sourcePrice, out error))
            {
                situation = null;
                return false;
            }

            if (!_client.TryGetPrice(snapshot.BuySymbol, _context.Trading.ReferenceSymbol, cancellationToken, out var targetPrice, out error))
            {
                situation = null;
                return false;
            }

            if (!_client.TryGetTradeFees(snapshot.SellSymbol, _context.Trading.ReferenceSymbol, cancellationToken, out var sourceMakerFee, out var _, out error))
            {
                situation = null;
                return false;
            }
            
            if (!_client.TryGetTradeFees(snapshot.BuySymbol, _context.Trading.ReferenceSymbol, cancellationToken, out var _, out var destinationTakerFee, out error))
            {
                situation = null;
                return false;
            }

            if (!_client.TryGetTrend(snapshot.SellSymbol, _context.Trading.ReferenceSymbol, _context.Trading.RsiPeriod, cancellationToken, out var sellTrend, out error))
            {
                situation = null;
                return false;
            }
            if (!_client.TryGetTrend(snapshot.BuySymbol, _context.Trading.ReferenceSymbol, _context.Trading.RsiPeriod, cancellationToken, out var buyTrend, out error))
            {
                situation = null;
                return false;
            }

            using var data = new DataContext();
            var lastSourcePurchase = data.FindLastPurchase(snapshot.SellSymbol, _context.Trading);
            var sourceDelta = new Delta
            {
                Symbol = snapshot.SellSymbol,
                PastPrice = lastSourcePurchase?.BuyPrice ?? sourcePrice,
                PastQuantity = lastSourcePurchase?.BuyQuantity ?? 0,
                PresentPrice = sourcePrice,
            };

            var lastTargetSell = data.FindLastSell(snapshot.BuySymbol, _context.Trading);
            var targetDelta = new Delta
            {
                Symbol = snapshot.BuySymbol,
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
                IsInitialCycle = lastSourcePurchase == null || lastTargetSell == null 
            };

            error = null;
            return true;
        }
        
        
        private void SaveAndReplaceSnapshot(CircularTradeSnapshot snapshot, EtAlii.BinanceMagic.Transaction transaction)
        {
            snapshot.SellSymbol = transaction.Sell.SymbolName;
            snapshot.SellPrice = transaction.Sell.QuoteQuantity;
            snapshot.SellQuantity = transaction.Sell.Quantity;

            snapshot.BuySymbol = transaction.Buy.SymbolName;
            snapshot.BuyPrice = transaction.Buy.QuoteQuantity;
            snapshot.BuyQuantity = transaction.Buy.Quantity;

            snapshot.Result = $"Transaction done";
            snapshot.LastCheck = _timeManager.GetNow();
            snapshot.LastSuccess = _timeManager.GetNow(); 
                    
            using var data = new DataContext();
            data.Attach(_context.Trading);
            
            snapshot.Profit = snapshot.SellPrice - snapshot.BuyPrice;
            
            _context.Trading.TotalProfit = snapshot.Profit + data.GetTotalProfits(_context.Trading);
            
            _context.RaiseChanged(AlgorithmChange.Important);
                    
            _context.Snapshot = snapshot = snapshot.ShallowClone();

            data.Entry(_context.Trading).State = EntityState.Modified;
            data.Entry(snapshot).State = EntityState.Added;
            data.SaveChanges();
        }
    }
}