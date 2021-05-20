#nullable enable
#pragma warning disable SL2001

namespace EtAlii.BinanceMagic.Surfing
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public partial class Sequence : SequenceBase, ISequence
    {
        private readonly AlgorithmSettings _settings;
        private readonly IClient _client;
        private readonly Data _data;
        private readonly ITimeManager _timeManager;
        private readonly TradeDetails _details;
        
        public IAlgorithmContext<object> Status => _status;
        private readonly IAlgorithmContext<object> _status;

        private CancellationToken _cancellationToken;
        private Situation? _situation;
        private Trend? _currentCoinTrend;
        private Trend? _bestCoinTrend;
        private Symbol? _symbolsSold;
        private Symbol? _symbolsBought;

        public Sequence(
            AlgorithmSettings settings, 
            IClient client, 
            IOutput output, 
            ITimeManager timeManager, 
            IPersistence<Transaction> persistence)
        {
            _settings = settings;
            _client = client;
            _timeManager = timeManager;
            
            _details = new TradeDetails
            {
                PayoutSymbol = _settings.PayoutCoin, 
                CurrentSymbol = _settings.PayoutCoin,
                CurrentVolume = _settings.InitialPurchase,
            };
            _status = new AlgorithmContext(output, _details, settings);
            _data = new Data(client, settings, persistence);
        }

        public void Run(CancellationToken cancellationToken, out bool keepRunning)
        {
            keepRunning = true;
            Task.Delay(TimeSpan.FromSeconds(30), cancellationToken).Wait(cancellationToken);
        }
        public void Initialize(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            Start();
        }

        protected override void OnStartEntered()
        {
            _data.Load();

            var lastTransaction = _data.Transactions.LastOrDefault();
            if (lastTransaction != null)
            {
                _details.CurrentVolume = lastTransaction.Buy.Quantity;
                _details.CurrentSymbol = lastTransaction.Buy.SymbolName;
                _details.LastSuccess = lastTransaction.Moment;
                _details.LastProfit = lastTransaction.Profit;
                _details.TotalProfit = lastTransaction.Buy.Quantity * lastTransaction.Buy.Price - _settings.InitialPurchase;
            }
            
            Continue();
        }

        protected override void OnGetSituationEntered()
        {
            _details.NextCheck = DateTime.MinValue;
            _details.Status = "Fetching situation...";
            _status.RaiseChanged();

            if(!_data.TryGetSituation(_cancellationToken, _details, out _situation, out string error))
            {
                _details.Status = error;
                _status.RaiseChanged();
                Error();
                return;
            }

            _details.Trends = _situation.Trends;
            _details.Status = "Fetching situation: Done";
            _status.RaiseChanged();

            Continue();
        }
   
        
        /// <summary>
        /// Implement this method to handle the entry of the 'DetermineOtherCoinValue' state.
        /// </summary>
        protected override void OnDetermineCoinToBetOnEnteredFromContinueTrigger(DetermineCoinToBetOnEventArgs e)
        {
            _details.Status = "Determining best trend...";
            _status.RaiseChanged();

            _currentCoinTrend = _situation!.Trends.SingleOrDefault(t => t.Symbol == _situation.CurrentCoin);
            _bestCoinTrend = _situation.Trends.OrderByDescending(t => t.Rsi).First();

            if (_currentCoinTrend == null)
            {
                if (_bestCoinTrend.Rsi >= _settings.RsiOverBought)
                {
                    // No coin. we need to select one.
                    _details.Status = "Determining best trend: No previous coin";
                    _status.RaiseChanged();
                    e.NoPreviousCoin();
                }
                else
                {
                    _details.Status = "Determining best trend: No coin has upward trend";
                    _status.RaiseChanged();
                    e.CurrentCoinHasBestTrend();
                }
            }
            else
            {
                if (_currentCoinTrend.Symbol == _bestCoinTrend.Symbol && _bestCoinTrend.Rsi >= _settings.RsiOverSold)
                {
                    _details.Status = "Determining best trend: Current coin has best trend";
                    _status.RaiseChanged();
                    e.CurrentCoinHasBestTrend();
                }
                // if (_currentCoinTrend.Rsi >= _settings.RsiOverBought)
                // {
                //     _details.Status = "Determining best trend: Current coin has best trend";
                //     _status.RaiseChanged();
                //     e.CurrentCoinHasBestTrend();
                // }
                else 
                {
                    //if (_bestCoinTrend.Rsi > _currentCoinTrend.Rsi &&  > _bestCoinTrend.Rsi >= _settings.RsiOverSold)
                    if (_bestCoinTrend.Rsi > _currentCoinTrend.Rsi && _bestCoinTrend.Rsi >= _settings.RsiOverBought)
                    {
                        // The current coin does no longer have the best trend. Let's dump it.
                        _details.Status = "Determining best trend: Other coin has better trend";
                        _status.RaiseChanged();
                        e.OtherCoinHasBetterTrend();
                    }
                    else
                    {
                        _details.Status = "Determining best trend: All coins have downward trends";
                        _status.RaiseChanged();
                        e.AllCoinsHaveDownwardTrends();
                    }
                }
            }
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'SellCurrentCoinInUsdtTransfer' state.
        /// </summary>
        protected override void OnSellCurrentCoinInUsdtTransferExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'Wait' state.
        /// </summary>
        protected override void OnWaitEntered()
        {
            _details.Status = null;
            _details.NextCheck = DateTime.Now + _settings.ActionInterval;
            _status.RaiseChanged();
            Task.Delay(_settings.ActionInterval, _cancellationToken).Wait(_cancellationToken);
            Continue();
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'Wait' state.
        /// </summary>
        protected override void OnWaitExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// DetermineOtherCoinValue --&gt; Wait : CurrentCoinHasBestTrend<br/>
        /// </summary>
        protected override void OnWaitEnteredFromCurrentCoinHasBestTrendTrigger()
        {
        }
    }
}