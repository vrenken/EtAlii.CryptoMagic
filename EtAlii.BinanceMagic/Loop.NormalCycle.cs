﻿namespace EtAlii.BinanceMagic
{
    using System;
    using System.Threading;

    public partial class Loop
    {
        private bool TryHandleNormalCycle(CancellationToken cancellationToken, Situation situation, out bool targetSucceeded)
        {
            targetSucceeded = false;

            if (_algorithm.TransactionIsWorthIt(situation, out var sellAction, out var buyAction))
            {
                _details.Result = $"Preparing to sell {sellAction.Quantity} {sellAction.Coin} and buy {buyAction.Quantity} {buyAction.Coin}";
                // ConsoleOutput.WriteFormatted("Preparing sell action : {0, -40} (= {1})", $"{sellAction.Quantity} {sellAction.Coin}", $"{sellAction.Price} {_settings.ReferenceCoin}");
                // ConsoleOutput.WriteFormatted("Preparing buy action  : {0, -40} (= {1})", $"{buyAction.Quantity} {buyAction.Coin}", $"{buyAction.Price} {_settings.ReferenceCoin}");

                _details.Result = "Feasible transaction found - Converting...";
                
                if (_client.TryConvert(sellAction, buyAction, _settings.ReferenceCoin, _details, cancellationToken, out var transaction))
                {
                    _details.Result = $"Transaction done!";
                    _details.LastSuccess = DateTime.Now; 
                    
                    _data.AddTransaction(transaction);
                    targetSucceeded = true;
                }
            }

            return targetSucceeded;
        }
    }
}