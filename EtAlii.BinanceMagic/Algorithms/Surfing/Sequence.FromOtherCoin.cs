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
                From = new Coin
                {
                    Symbol = _currentCoinTrend!.Coin,
                    Price = _currentCoinTrend.Price,
                    Quantity = 0,
                },
                To = new Coin
                {
                    Symbol = _bestCoinTrend!.Coin,
                    Price = _bestCoinTrend.Price,
                    Quantity = 0,
                },
                Profit = 0,
                Target = 0,
            };
            _data.AddTransaction(transaction);
            
            _details.Status = null;
            _details.Step += 1;
            _details.LastSuccess = now;
            _details.CurrentCoin = _bestCoinTrend!.Coin;
            _status.RaiseChanged();
        }
    }
}