// namespace EtAlii.CryptoMagic.Surfing
// {
//     using System;
//
//     public partial class Sequence
//     {
//         /// <summary>
//         /// Implement this method to handle the transition below:<br/>
//         /// DetermineOtherCoinValue --&gt; TransferToOtherCoin : OtherCoinHasBetterTrend<br/>
//         /// </summary>
//         protected override void OnTransferToOtherCoinEntered()
//         {
//             _details.Status = $"{_bestCoinTrend!.Symbol} has a better future - switching...";
//             _status.RaiseChanged();
//         }
//
//
//         /// <summary>
//         /// Implement this method to handle the transition below:<br/>
//         /// _Begin --&gt; DetermineSymbolPair : _BeginToDetermineSymbolPair<br/>
//         /// </summary>
//         protected override void OnDetermineSymbolPairEntered(DetermineSymbolPairEventArgs e)
//         {
//             e.IsNoSymbolPair();
//         }
//         
//         /// <summary>
//         /// Implement this method to handle the exit of the 'TransferToOtherCoin' state.
//         /// </summary>
//         protected override void OnTransferToOtherCoinExited()
//         {
//             var now = DateTime.Now;
//             var transaction = new TradeTransaction
//             {
//                 Moment = now,
//                 Sell = _symbolsSold,
//                 Buy = _symbolsBought,
//                 Profit = _symbolsSold!.QuoteQuantity - _symbolsBought!.QuoteQuantity,
//                 Target = 0,
//             };
//             _data.AddTransaction(transaction);
//             
//             _details.Status = null;
//             _details.Step += 1;
//             _details.LastSuccess = now;
//             _details.LastProfit = transaction.Profit;
//             _details.TotalProfit = _symbolsBought!.Quantity * _symbolsBought.Price - _settings.InitialPurchase;
//             _details.CurrentSymbol = _bestCoinTrend!.Symbol;
//             _details.CurrentVolume = _symbolsBought!.Quantity;
//             _status.RaiseChanged();
//         }
//     }
// }