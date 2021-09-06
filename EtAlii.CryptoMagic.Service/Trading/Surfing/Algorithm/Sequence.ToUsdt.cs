// #nullable enable
// namespace EtAlii.CryptoMagic.Service.Surfing
// {
//     public partial class Sequence
//     {
//         /// <summary>
//         /// Implement this method to handle the entry of the 'TransferToUsdt' state.
//         /// </summary>
//         protected override void OnTransferToUsdtEntered()
//         {
//         }
//
//         protected override void OnSellCurrentCoinInUsdtTransferEntered()
//         {
//             _details!.Status = $"Selling {_details.CurrentSymbol}/{_details.PayoutSymbol}...";
//             _status!.RaiseChanged();
//             
//             var sellAction = new SellAction
//             {
//                 Symbol = _currentCoinTrend!.Symbol,
//                 Quantity = _details.CurrentVolume * _settings!.TransferFactor,
//                 TransactionId = "Surfing" 
//             };
//             
//             if (!_client!.TrySell(sellAction, _settings.PayoutCoin, _cancellationToken, _timeManager!.GetNow, out _symbolsSold, out var error))
//             {
//                 _details.Status = error;
//                 _status.RaiseChanged();
//                 return;
//             }
//
//             Continue();
//         }
//
//         protected override void OnWaitUntilCoinSoldInUsdtTransferEntered()
//         {
//             _details!.Status = $"Selling {_details.CurrentSymbol}/{_details.PayoutSymbol}: Waiting for confirmation...";
//             _status!.RaiseChanged();
//
//             Continue();
//         }
//         
//         /// <summary>
//         /// Implement this method to handle the exit of the 'TransferToUsdt' state.
//         /// </summary>
//         protected override void OnTransferToUsdtExited()
//         {
//             var now = _timeManager.GetNow();
//             var transaction = new TradeTransaction
//             {
//                 Moment = now,
//                 Sell = _symbolsSold,
//                 Buy = new Symbol
//                 {
//                     SymbolName = _details.PayoutSymbol,
//                     QuoteQuantity = 0,
//                     Quantity = 0,
//                 },
//                 Profit = 0,
//                 Target = 0,
//             };
//             _data.AddTransaction(transaction);
//             
//             _details.Status = null;
//             _details.Step += 1;
//             _details.LastSuccess = now;
//             _details.TotalProfit = _symbolsSold!.QuoteQuantity - _settings.InitialPurchase;
//             _details.CurrentSymbol = _settings.PayoutCoin;
//             _details.CurrentVolume =  _symbolsSold!.QuoteQuantity;
//             _status.RaiseChanged();
//         }
//     }
// }