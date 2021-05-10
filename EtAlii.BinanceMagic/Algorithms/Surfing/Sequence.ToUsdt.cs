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

        protected override void OnSellCurrentCoinInUsdtTransferEntered()
        {
            _details!.Status = $"Selling {_details.CurrentCoin}/{_details.PayoutCoin}...";
            _status!.RaiseChanged();
            
            var sellAction = new SellAction
            {
                Coin = _currentCoinTrend!.Coin,
                Quantity = _details.CurrentVolume * _settings!.TransferFactor,
                TransactionId = "Surfing" 
            };
            
            if (!_client!.TrySell(sellAction, _settings.PayoutCoin, _cancellationToken, _timeManager!.GetNow, out _coinsSold, out var error))
            {
                _details.Status = error;
                _status.RaiseChanged();
                return;
            }

            Continue();
        }

        protected override void OnWaitUntilCoinSoldInUsdtTransferEntered()
        {
            _details!.Status = $"Selling {_details.CurrentCoin}/{_details.PayoutCoin}: Waiting for confirmation...";
            _status!.RaiseChanged();

            Continue();
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'TransferToUsdt' state.
        /// </summary>
        protected override void OnTransferToUsdtExited()
        {
            var now = _timeManager.GetNow();
            var transaction = new Transaction
            {
                Moment = now,
                From = _coinsSold,
                To = new Coin
                {
                    Symbol = _details.PayoutCoin,
                    QuoteQuantity = 0,
                    Quantity = 0,
                },
                Profit = 0,
                Target = 0,
            };
            _data.AddTransaction(transaction);
            
            _details.Status = null;
            _details.Step += 1;
            _details.LastSuccess = now;
            _details.TotalProfit = _coinsSold!.QuoteQuantity - _settings.InitialPurchase;
            _details.CurrentCoin = _settings.PayoutCoin;
            _details.CurrentVolume =  _coinsSold!.QuoteQuantity;
            _status.RaiseChanged();
        }
    }
}