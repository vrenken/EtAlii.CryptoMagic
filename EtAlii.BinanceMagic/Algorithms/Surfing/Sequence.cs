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
        private readonly Data _data;

        public IStatusProvider Status => _status;
        private readonly StatusProvider _status;

        private CancellationToken _cancellationToken;
        private readonly TradeDetails _details;

        private Situation? _situation;
        private Trend? _currentCoinTrend;
        private Trend? _bestCoinTrend;

        public Sequence(AlgorithmSettings settings, IClient client, IOutput output)
        {
            _settings = settings;
            _details = new TradeDetails
            {
                PayoutCoin = _settings.PayoutCoin, 
                CurrentCoin = _settings.PayoutCoin,
                CurrentVolume = _settings.InitialPurchase,
            };
            _status = new StatusProvider(output, _details);
            _data = new Data(client, settings, output);
        }

        public void Run(CancellationToken cancellationToken)
        {
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
                _details.CurrentVolume = lastTransaction.To.Quantity;
                _details.CurrentCoin = lastTransaction.To.Symbol;
                _details.LastSuccess = lastTransaction.Moment;
                _details.LastProfit = lastTransaction.Profit;
                _details.TotalProfit = _data.Transactions.Sum(t => t.Profit);
            }
            
            Continue();
        }

        protected override void OnGetSituationEntered()
        {
            _details.NextCheck = DateTime.MinValue;
            _details.Status = "Fetching situation...";
            _status.RaiseChanged();

            if(!_data.TryGetSituation(_cancellationToken, _details, out _situation))
            {
                Error();
                return;
            }

            _details.Trends = _situation.Trends;
            _status.RaiseChanged();

            Continue();
        }
   
        
        /// <summary>
        /// Implement this method to handle the entry of the 'DetermineOtherCoinValue' state.
        /// </summary>
        protected override void OnDetermineCoinToBetOnEnteredFromContinueTrigger(DetermineCoinToBetOnEventArgs e)
        {
            _currentCoinTrend = _situation!.Trends.SingleOrDefault(t => t.Coin == _situation.CurrentCoin);
            _bestCoinTrend = _situation.Trends.OrderByDescending(t => t.Change).First();

            if (_situation.Trends.All(t => t.Change <= 0))
            {
                if (_currentCoinTrend != null)
                {
                    e.AllCoinsHaveDownwardTrends();
                }
                else
                {
                    e.CurrentCoinHasBestTrend();
                }
            }
            else if (_currentCoinTrend == null)
            {
                // No coin. we need to select one.
                e.NoPreviousCoin();
            }
            else
            {
                if (_bestCoinTrend.Coin == _situation.CurrentCoin)
                {
                    // The current coin still has the best trend. Let's stick with it.
                    _details.Status = "No better situation found";
                    _status.RaiseChanged();
                    e.CurrentCoinHasBestTrend();
                }
                else
                {
                    // The current coin does no longer have the best trend. Let's dump it.
                    e.OtherCoinHasBetterTrend();
                }
            }
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'SellCurrentCoinInUsdtTransfer' state.
        /// </summary>
        protected override void OnSellCurrentCoinInUsdtTransferEntered()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'SellCurrentCoinInUsdtTransfer' state.
        /// </summary>
        protected override void OnSellCurrentCoinInUsdtTransferExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// _Begin --&gt; SellCurrentCoinInUsdtTransfer : _BeginToSellCurrentCoinInUsdtTransfer<br/>
        /// </summary>
        protected override void OnSellCurrentCoinInUsdtTransferEnteredFrom_BeginToSellCurrentCoinInUsdtTransferTrigger()
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