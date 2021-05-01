namespace EtAlii.BinanceMagic
{
    using System;

    public class StatusProvider : IStatusProvider
    {
        private readonly IOutput _output;
        private readonly TradeDetails _details;

        public event Action<StatusInfo> Changed;
        
        public StatusProvider(IOutput output, TradeDetails details)
        {
            _output = output;
            _details = details;
        }

        public void RaiseChanged(StatusInfo statusInfo = StatusInfo.Normal) => Changed?.Invoke(statusInfo);
        
        public void Write()
        {
            var nextCheck = _details.NextCheck != DateTime.MinValue ? _details.NextCheck.ToString("yyyy-MM-dd HH:mm:ss") : "Now...";
            var lastSuccess = _details.LastSuccess != DateTime.MinValue ? _details.LastSuccess.ToString("yyyy-MM-dd HH:mm:ss") : "None";

            var differencePrefix = _details.Difference > 0 ? "+" : "";
            _output.WriteLine("");
            _output.WriteLine($"{_details.SellCoin}->{_details.BuyCoin}");
            WriteColumns($"Sell         : +{_details.SellPrice:000.000000000} {_details.ReferenceCoin}",                  $"Quantity : +{_details.SellQuantity:000.000000000} {_details.SellCoin}",   $"Trend : {_details.SellTrend:+000.000;-000.000}", _details.SellPriceIsAboveNotionalMinimum ? InfoType.Positive : InfoType.Negative);
            WriteColumns($"Buy          : -{_details.BuyPrice:000.000000000} {_details.ReferenceCoin}",                   $"Quantity : -{_details.BuyQuantity:000.000000000} {_details.BuyCoin}",     $"Trend : {_details.BuyTrend:+000.000;-000.000}", _details.BuyPriceIsAboveNotionalMinimum ? InfoType.Positive : InfoType.Negative);
            WriteColumns($"Diff         : {differencePrefix}{_details.Difference:000.000000000} {_details.ReferenceCoin}",$"Target   : +{_details.Target:000.000000000} {_details.ReferenceCoin}", _details.SufficientProfit ? InfoType.Positive : InfoType.Negative);
            WriteColumns($"Last success : {lastSuccess}",                                                                          $"Profit   : +{_details.Profit:000.000000000} {_details.ReferenceCoin}");
            WriteColumns($"Last check   : {_details.LastCheck:yyyy-MM-dd HH:mm:ss}",                                               $"Result   : {_details.Result}");
            WriteColumns($"Next check   : {nextCheck}",                                                                            $"Step     : {_details.Step}");
        }

        private void WriteColumns(string first, string second, InfoType firstInfoType = InfoType.Normal, InfoType secondInfoType = InfoType.Normal)
        {
            Write($"{first,-40}", firstInfoType);
            _output.Write(" ");
            Write(second, secondInfoType);
            _output.Write(Environment.NewLine);
        }

        private void WriteColumns(string first, string second, string third, InfoType firstInfoType = InfoType.Normal, InfoType secondInfoType = InfoType.Normal, InfoType thirdInfoType = InfoType.Normal)
        {
            Write($"{first,-40}", firstInfoType);
            _output.Write(" ");
            Write($"{second,-35}", secondInfoType);
            _output.Write(" ");
            Write($"{third,-20}", thirdInfoType);
            _output.Write(Environment.NewLine);
        }

        private void Write(string text, InfoType infoType)
        {
            var color = Console.ForegroundColor;
            if (infoType != InfoType.Normal)
            {
                Console.ForegroundColor = infoType == InfoType.Positive
                    ? ConsoleColor.Green
                    : ConsoleColor.Red;
            }
            _output.Write(text);
            Console.ForegroundColor = color;
        }
    }

}