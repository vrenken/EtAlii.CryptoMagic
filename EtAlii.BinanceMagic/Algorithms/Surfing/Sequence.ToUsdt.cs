#nullable enable
namespace EtAlii.BinanceMagic.Surfing
{
    public partial class Sequence
    {
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
    }
}