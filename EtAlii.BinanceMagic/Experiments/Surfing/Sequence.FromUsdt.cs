namespace EtAlii.BinanceMagic.Surfing
{
    using System;

    public partial class Sequence
    {
        protected override void OnTransferFromUsdtEntered()
        {
            _details.Status = $"Initial purchase {_bestCoinTrend!.Coin}...";
            _status.RaiseChanged();
        }

        protected override void OnBuyCurrentCoinInUsdtTransferEntered()
        {
            _details.Status = $"Buying {_bestCoinTrend!.Coin}/{_details.PayoutCoin}...";
            _status.RaiseChanged();

            var buyAction = new BuyAction
            {
                Coin = _bestCoinTrend.Coin,
                Price = _details.CurrentVolume * _settings.TransferFactor,
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

        protected override void OnWaitUntilCoinBoughtInUsdtTransferEntered()
        {
            _details.Status = $"Buying {_bestCoinTrend!.Coin}/{_details.PayoutCoin}: Waiting for confirmation...";
            _status.RaiseChanged();
            
            Continue();
            
        }

        protected override void OnWaitUntilCoinBoughtInUsdtTransferExited()
        {
            var now = _timeManager.GetNow();
            var transaction = new Transaction
            {
                Moment = now,
                From = new Coin
                {
                    Symbol = _details.PayoutCoin,
                    QuoteQuantity = 0,
                    Quantity = 0,
                },
                To = _coinsBought,
                Profit = 0,
                Target = 0,
            };
            _data.AddTransaction(transaction);
            
            _details.Status = null;
            _details.Step += 1;
            _details.LastSuccess = now;
            _details.TotalProfit = _coinsBought!.Quantity * _coinsBought.Price - _settings.InitialPurchase;
            _details.CurrentCoin = _bestCoinTrend!.Coin;
            _details.CurrentVolume = _coinsBought!.Quantity;
            _status.RaiseChanged();
        }
    }
}