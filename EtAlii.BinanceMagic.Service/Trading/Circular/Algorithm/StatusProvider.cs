namespace EtAlii.BinanceMagic.Service
{
    using System;

    public class StatusProvider : IStatusProvider
    {
        private readonly IOutput _output;
        public CircularTradeSnapshot Snapshot
        {
            get => _snapshot;
            set
            {
                if (_snapshot != value)
                {
                    _snapshot = value;
                    RaiseChanged();
                }
            }
        }

        private CircularTradeSnapshot _snapshot;

        public event Action<StatusInfo> Changed;
        
        public StatusProvider(IOutput output)
        {
            _output = output;
        }

        public void RaiseChanged(StatusInfo statusInfo = StatusInfo.Normal) => Changed?.Invoke(statusInfo);
        
        public void Write()
        {
            var nextCheck = _snapshot.NextCheck != DateTime.MinValue ? _snapshot.NextCheck.ToString("yyyy-MM-dd HH:mm:ss") : "Now...";
            var lastSuccess = _snapshot.LastSuccess != DateTime.MinValue ? _snapshot.LastSuccess.ToString("yyyy-MM-dd HH:mm:ss") : "None";

            var differencePrefix = _snapshot.Difference > 0 ? "+" : "";
            _output.WriteLine("");
            _output.WriteLine($"{_snapshot.SellSymbol}->{_snapshot.BuySymbol}");
            WriteColumns($"Sell         : +{_snapshot.SellPrice:000.000000000} {_snapshot.ReferenceSymbol}",                  $"Quantity : +{_snapshot.SellQuantity:000.000000000} {_snapshot.SellSymbol}",   $"Trend : {_snapshot.SellTrend:+000.000;-000.000}", _snapshot.SellPriceIsAboveNotionalMinimum ? InfoType.Positive : InfoType.Negative);
            WriteColumns($"Buy          : -{_snapshot.BuyPrice:000.000000000} {_snapshot.ReferenceSymbol}",                   $"Quantity : -{_snapshot.BuyQuantity:000.000000000} {_snapshot.BuySymbol}",     $"Trend : {_snapshot.BuyTrend:+000.000;-000.000}", _snapshot.BuyPriceIsAboveNotionalMinimum ? InfoType.Positive : InfoType.Negative);
            WriteColumns($"Diff         : {differencePrefix}{_snapshot.Difference:000.000000000} {_snapshot.ReferenceSymbol}",$"Target   : +{_snapshot.Target:000.000000000} {_snapshot.ReferenceSymbol}", _snapshot.SufficientProfit ? InfoType.Positive : InfoType.Negative);
            WriteColumns($"Last success : {lastSuccess}",                                                                          $"Profit   : +{_snapshot.Profit:000.000000000} {_snapshot.ReferenceSymbol}");
            WriteColumns($"Last check   : {_snapshot.LastCheck:yyyy-MM-dd HH:mm:ss}",                                               $"Result   : {_snapshot.Result}");
            WriteColumns($"Next check   : {nextCheck}",                                                                            $"Step     : {_snapshot.Step}");
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