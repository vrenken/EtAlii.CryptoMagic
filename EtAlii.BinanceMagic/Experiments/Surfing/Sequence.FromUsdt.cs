namespace EtAlii.BinanceMagic.Surfing
{
    using System;

    public partial class Sequence
    {
        protected override void OnTransferFromUsdtEntered()
        {
            _details.Status = $"Initial purchase {_bestCoinTrend!.Symbol}...";
            _status.RaiseChanged();
        }

        protected override void OnBuyCurrentCoinInUsdtTransferEntered()
        {
            _details.Status = $"Buying {_bestCoinTrend!.Symbol}/{_details.PayoutSymbol}...";
            _status.RaiseChanged();

            var buyAction = new BuyAction
            {
                Symbol = _bestCoinTrend.Symbol,
                QuotedQuantity = _details.CurrentVolume * _settings.TransferFactor,
                TransactionId = "Surfing" 
            };
            
            if (!_client.TryBuy(buyAction, _settings.PayoutCoin, _cancellationToken, _timeManager.GetNow, out _symbolsBought, out var error))
            {
                _details.Status = error;
                _status.RaiseChanged();
                return;
            }
                
            Continue();
        }

        protected override void OnWaitUntilCoinBoughtInUsdtTransferEntered()
        {
            _details.Status = $"Buying {_bestCoinTrend!.Symbol}/{_details.PayoutSymbol}: Waiting for confirmation...";
            _status.RaiseChanged();
            
            Continue();
            
        }

        protected override void OnWaitUntilCoinBoughtInUsdtTransferExited()
        {
            var now = _timeManager.GetNow();
            var transaction = new Transaction
            {
                Moment = now,
                Sell = new Symbol
                {
                    SymbolName = _details.PayoutSymbol,
                    QuoteQuantity = 0,
                    Quantity = 0,
                },
                Buy = _symbolsBought,
                Profit = 0,
                Target = 0,
            };
            _data.AddTransaction(transaction);
            
            _details.Status = null;
            _details.Step += 1;
            _details.LastSuccess = now;
            _details.TotalProfit = _symbolsBought!.Quantity * _symbolsBought.Price - _settings.InitialPurchase;
            _details.CurrentSymbol = _bestCoinTrend!.Symbol;
            _details.CurrentVolume = _symbolsBought!.Quantity;
            _status.RaiseChanged();
        }
    }
}