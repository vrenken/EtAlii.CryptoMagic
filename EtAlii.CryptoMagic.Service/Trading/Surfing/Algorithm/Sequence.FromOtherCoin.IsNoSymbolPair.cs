// #nullable enable
// namespace EtAlii.CryptoMagic.Service.Surfing
// {
//     public partial class Sequence
//     {
//    
//         /// <summary>
//         /// Implement this method to handle the transition below:<br/>
//         /// DetermineSymbolPair --&gt; SellCurrentCoin : IsNoSymbolPair<br/>
//         /// </summary>
//         protected override void OnSellCurrentCoinEntered()
//         {
//             _details.Status = $"Selling {_details.CurrentSymbol}/{_details.PayoutSymbol}...";
//             _status.RaiseChanged();
//             
//             var sellAction = new SellAction
//             {
//                 Symbol = _currentCoinTrend!.Symbol,
//                 Quantity = _details.CurrentVolume * _settings.TransferFactor,
//                 TransactionId = "Surfing" 
//             };
//             
//             if (!_client.TrySell(sellAction, _settings.PayoutCoin, _cancellationToken, _timeManager.GetNow, out _symbolsSold, out var error))
//             {
//                 _details.Status = error;
//                 _status.RaiseChanged();
//                 return;
//             }
//
//             Continue();
//         }
//
//         protected override void OnWaitUntilCoinSoldEntered()
//         {
//             _details.Status = $"Selling {_details.CurrentSymbol}/{_details.PayoutSymbol}: Waiting for confirmation...";
//             _status.RaiseChanged();
//
//             Continue();
//         }
//                 
//         /// <summary>
//         /// Implement this method to handle the entry of the 'BuyOtherCoin' state.
//         /// </summary>
//         protected override void OnBuyOtherCoinEntered()
//         {
//             _details.Status = $"Buying {_bestCoinTrend!.Symbol}/{_details.PayoutSymbol}...";
//             _status.RaiseChanged();
//
//             var buyAction = new BuyAction
//             {
//                 Symbol = _bestCoinTrend.Symbol,
//                 QuotedQuantity = _symbolsSold!.QuoteQuantity * _settings.TransferFactor,
//                 TransactionId = "Surfing" 
//             };
//             
//             if (!_client.TryBuy(buyAction, _settings.PayoutCoin, _cancellationToken, _timeManager.GetNow, out _symbolsBought, out var error))
//             {
//                 _details.Status = error;
//                 _status.RaiseChanged();
//                 return;
//             }
//
//             Continue();
//         }
//
//         
//         
//         /// <summary>
//         /// Implement this method to handle the entry of the 'BuyOtherCoin' state.
//         /// </summary>
//         protected override void OnWaitUntilCoinBoughtEntered()
//         {
//             _details.Status = $"Buying {_bestCoinTrend!.Symbol}/{_details.PayoutSymbol}: Waiting for confirmation...";
//             _status.RaiseChanged();
//
//             Continue();
//         }
//     }
// }