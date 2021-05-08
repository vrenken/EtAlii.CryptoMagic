#nullable enable
namespace EtAlii.BinanceMagic.Surfing
{
    public partial class Sequence
    {
   
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// DetermineSymbolPair --&gt; SellCurrentCoin : IsNoSymbolPair<br/>
        /// </summary>
        protected override void OnSellCurrentCoinEntered()
        {
            _details.Status = $"Selling {_details.CurrentCoin}/{_details.PayoutCoin}...";
            _status.RaiseChanged();
            
            Continue();
        }

        protected override void OnWaitUntilCoinSoldEntered()
        {
            _details.Status = $"Selling {_details.CurrentCoin}/{_details.PayoutCoin}: Waiting for confirmation...";
            _status.RaiseChanged();

            Continue();
        }
                
        /// <summary>
        /// Implement this method to handle the entry of the 'BuyOtherCoin' state.
        /// </summary>
        protected override void OnBuyOtherCoinEntered()
        {
            _details.Status = $"Buying {_bestCoinTrend!.Coin}/{_details.PayoutCoin}...";
            _status.RaiseChanged();

            Continue();
        }

        
        
        /// <summary>
        /// Implement this method to handle the entry of the 'BuyOtherCoin' state.
        /// </summary>
        protected override void OnWaitUntilCoinBoughtEntered()
        {
            _details.Status = $"Buying {_bestCoinTrend!.Coin}/{_details.PayoutCoin}: Waiting for confirmation...";
            _status.RaiseChanged();

            Continue();
        }
    }
}