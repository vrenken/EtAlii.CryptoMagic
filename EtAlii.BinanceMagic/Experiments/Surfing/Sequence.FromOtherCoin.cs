namespace EtAlii.BinanceMagic.Surfing
{
    using System;

    public partial class Sequence
    {
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// DetermineOtherCoinValue --&gt; TransferToOtherCoin : OtherCoinHasBetterTrend<br/>
        /// </summary>
        protected override void OnTransferToOtherCoinEntered()
        {
            _details.Status = $"{_bestCoinTrend!.Coin} has a better future - switching...";
            _status.RaiseChanged();
        }


        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// _Begin --&gt; DetermineSymbolPair : _BeginToDetermineSymbolPair<br/>
        /// </summary>
        protected override void OnDetermineSymbolPairEntered(DetermineSymbolPairEventArgs e)
        {
            e.IsNoSymbolPair();
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'TransferToOtherCoin' state.
        /// </summary>
        protected override void OnTransferToOtherCoinExited()
        {
            var now = DateTime.Now;
            var transaction = new Transaction
            {
                Moment = now,
                From = _coinsSold,
                To = _coinsBought,
                Profit = _coinsSold!.QuoteQuantity - _coinsBought!.QuoteQuantity,
                Target = 0,
            };
            _data.AddTransaction(transaction);
            
            _details.Status = null;
            _details.Step += 1;
            _details.LastSuccess = now;
            _details.LastProfit = transaction.Profit;
            _details.TotalProfit = _coinsBought!.Quantity * _coinsBought.Price - _settings.InitialPurchase;
            _details.CurrentCoin = _bestCoinTrend!.Coin;
            _details.CurrentVolume = _coinsBought!.Quantity;
            _status.RaiseChanged();
        }
    }
}