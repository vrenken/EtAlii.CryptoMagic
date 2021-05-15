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
            
            var sellAction = new SellAction
            {
                Coin = _currentCoinTrend!.Coin,
                Quantity = _details.CurrentVolume * _settings.TransferFactor,
                TransactionId = "Surfing" 
            };
            
            if (!_client.TrySell(sellAction, _settings.PayoutCoin, _cancellationToken, _timeManager.GetNow, out _coinsSold, out var error))
            {
                _details.Status = error;
                _status.RaiseChanged();
                return;
            }

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

            var buyAction = new BuyAction
            {
                Coin = _bestCoinTrend.Coin,
                Price = _coinsSold!.QuoteQuantity * _settings.TransferFactor,
                TransactionId = "Surfing" 
            };
            
            if (!_client.TryBuy(buyAction, _settings.PayoutCoin, _cancellationToken, _timeManager.GetNow, out _coinsBought, out var error))
            {
                _details.Status = error;
                _status.RaiseChanged();
                return;
            }

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