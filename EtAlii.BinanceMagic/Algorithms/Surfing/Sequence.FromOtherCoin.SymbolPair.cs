#nullable enable
namespace EtAlii.BinanceMagic.Surfing
{
    public partial class Sequence
    {
    
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// DetermineSymbolPair --&gt; SellAsSymbolPair : IsSymbolPair<br/>
        /// </summary>
        protected override void OnSellAsSymbolPairEntered()
        {
            _details.Status = $"Selling {_details.CurrentCoin}/{_details.PayoutCoin}...";
            _status.RaiseChanged();

            Continue();
        }
        
                
        /// <summary>
        /// Implement this method to handle the entry of the 'WaitUntilCoinSoldAsSymbolPair' state.
        /// </summary>
        protected override void OnWaitUntilCoinSoldAsSymbolPairEntered()
        {
            _details.Status = $"Selling {_details.CurrentCoin}/{_details.PayoutCoin}: Waiting for confirmation...";
            _status.RaiseChanged();

            Continue();
        }
    }
}