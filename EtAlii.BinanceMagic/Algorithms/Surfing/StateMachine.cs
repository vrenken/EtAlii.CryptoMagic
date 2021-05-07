namespace EtAlii.BinanceMagic.Surfing
{
    using System.Threading.Tasks;

    public class StateMachine : SurfingStateMachineBase
    {
        private readonly AlgorithmSettings _settings;
        private readonly Data _data;

        public StateMachine(AlgorithmSettings settings, Data data)
        {
            _settings = settings;
            _data = data;
        }

        protected override void OnStartEntered()
        {
            _data.Load();
            Continue();
        }

        protected override void OnGetTrendsEntered()
        {
        }
        /// <summary>
        /// Implement this method to handle the entry of the 'BuyOtherCoin' state.
        /// </summary>
        protected override void OnBuyOtherCoinEntered()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'BuyOtherCoin' state.
        /// </summary>
        protected override void OnBuyOtherCoinExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'DetermineOtherCoinValue' state.
        /// </summary>
        protected override void OnDetermineOtherCoinValueEntered(DetermineOtherCoinValueEventArgs e)
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'DetermineOtherCoinValue' state.
        /// </summary>
        protected override void OnDetermineOtherCoinValueExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'DetermineSymbolPair' state.
        /// </summary>
        protected override void OnDetermineSymbolPairEntered(DetermineSymbolPairEventArgs e)
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'DetermineSymbolPair' state.
        /// </summary>
        protected override void OnDetermineSymbolPairExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// _Begin --&gt; DetermineSymbolPair : _BeginToDetermineSymbolPair<br/>
        /// </summary>
        protected override void OnDetermineSymbolPairEnteredFrom_BeginToDetermineSymbolPairTrigger(DetermineSymbolPairEventArgs e)
        {
            //e.IsNoSymbolPair();
            e.IsSymbolPair();
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'SellAsSymbolPair' state.
        /// </summary>
        protected override void OnSellAsSymbolPairEntered()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'SellAsSymbolPair' state.
        /// </summary>
        protected override void OnSellAsSymbolPairExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// DetermineSymbolPair --&gt; SellAsSymbolPair : IsSymbolPair<br/>
        /// </summary>
        protected override void OnSellAsSymbolPairEnteredFromIsSymbolPairTrigger()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'SellCurrentCoin' state.
        /// </summary>
        protected override void OnSellCurrentCoinEntered()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'SellCurrentCoin' state.
        /// </summary>
        protected override void OnSellCurrentCoinExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// DetermineSymbolPair --&gt; SellCurrentCoin : IsNoSymbolPair<br/>
        /// </summary>
        protected override void OnSellCurrentCoinEnteredFromIsNoSymbolPairTrigger()
        {
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
        /// Implement this method to handle the entry of the 'TransferToOtherCoin' state.
        /// </summary>
        protected override void OnTransferToOtherCoinEntered()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'TransferToOtherCoin' state.
        /// </summary>
        protected override void OnTransferToOtherCoinExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// DetermineOtherCoinValue --&gt; TransferToOtherCoin : OtherCoinHasBetterTrend<br/>
        /// </summary>
        protected override void OnTransferToOtherCoinEnteredFromOtherCoinHasBetterTrendTrigger()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'TransferToUsdt' state.
        /// </summary>
        protected override void OnTransferToUsdtEntered()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'TransferToUsdt' state.
        /// </summary>
        protected override void OnTransferToUsdtExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// DetermineOtherCoinValue --&gt; TransferToUsdt : AllCoinsHaveDownwardTrends<br/>
        /// </summary>
        protected override void OnTransferToUsdtEnteredFromAllCoinsHaveDownwardTrendsTrigger()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'Wait' state.
        /// </summary>
        protected override void OnWaitEntered()
        {
            Task.Delay(_settings.ActionInterval).Wait();
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
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// TransferToUsdt --&gt; Wait : TransferToUsdtToWait<br/>
        /// </summary>
        protected override void OnWaitEnteredFromTransferToUsdtToWaitTrigger()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// TransferToOtherCoin --&gt; Wait : TransferToOtherCoinToWait<br/>
        /// </summary>
        protected override void OnWaitEnteredFromTransferToOtherCoinToWaitTrigger()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'WaitUntilCoinBought' state.
        /// </summary>
        protected override void OnWaitUntilCoinBoughtEntered()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'WaitUntilCoinBought' state.
        /// </summary>
        protected override void OnWaitUntilCoinBoughtExited()
        {
        }

        /// <summary>
        /// Implement this method to handle the exit of the 'WaitUntilCoinSold' state.
        /// </summary>
        protected override void OnWaitUntilCoinSoldExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'WaitUntilCoinSoldAsSymbolPair' state.
        /// </summary>
        protected override void OnWaitUntilCoinSoldAsSymbolPairEntered()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'WaitUntilCoinSoldAsSymbolPair' state.
        /// </summary>
        protected override void OnWaitUntilCoinSoldAsSymbolPairExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'WaitUntilCoinSoldInUsdtTransfer' state.
        /// </summary>
        protected override void OnWaitUntilCoinSoldInUsdtTransferEntered()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'WaitUntilCoinSoldInUsdtTransfer' state.
        /// </summary>
        protected override void OnWaitUntilCoinSoldInUsdtTransferExited()
        {
        }
    }
}