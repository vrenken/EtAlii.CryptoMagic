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
            var now = DateTime.Now;
            var transaction = new Transaction
            {
                Moment = now,
                From = new Coin
                {
                    Symbol = _details.PayoutCoin,
                    Price = 0,
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