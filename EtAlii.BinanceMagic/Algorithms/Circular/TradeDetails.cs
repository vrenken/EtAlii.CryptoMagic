namespace EtAlii.BinanceMagic
{
    using System;

    public class StatusWriter
    {
        private readonly IOutput _output;

        public StatusWriter(IOutput output)
        {
            _output = output;
        }

        public void Write(TradeDetails d)
        {
            var nextCheck = d.NextCheck != DateTime.MinValue ? d.NextCheck.ToString("yyyy-MM-dd HH:mm:ss") : "Now...";
            var lastSuccess = d.LastSuccess != DateTime.MinValue ? d.LastSuccess.ToString("yyyy-MM-dd HH:mm:ss") : "None";

            var differencePrefix = d.Difference > 0 ? "+" : "";
            Console.Clear();
            Console.WriteLine();
            _output.WriteLine($"{d.SellCoin}->{d.BuyCoin}");
            WriteColumns($"Sell         : +{d.SellPrice:000.000000000} {d.ReferenceCoin}",                      $"Quantity : +{d.SellQuantity:000.000000000} {d.SellCoin}", $"Trend : {d.SellTrend:+000.000;-000.000}", d.SellPriceIsAboveNotionalMinimum ? InfoType.Positive : InfoType.Negative);
            WriteColumns($"Buy          : -{d.BuyPrice:000.000000000} {d.ReferenceCoin}",                       $"Quantity : -{d.BuyQuantity:000.000000000} {d.BuyCoin}", $"Trend : {d.BuyTrend:+000.000;-000.000}", d.BuyPriceIsAboveNotionalMinimum ? InfoType.Positive : InfoType.Negative);
            WriteColumns($"Diff         : {differencePrefix}{d.Difference:000.000000000} {d.ReferenceCoin}",    $"Target   : +{d.Target:000.000000000} {d.ReferenceCoin}", d.SufficientProfit ? InfoType.Positive : InfoType.Negative);
            WriteColumns($"Last success : {lastSuccess}",                                                                $"Profit   : +{d.Profit:000.000000000} {d.ReferenceCoin}");
            WriteColumns($"Last check   : {d.LastCheck:yyyy-MM-dd HH:mm:ss}",                                            $"Result   : {d.Result}");
            WriteColumns($"Next check   : {nextCheck}",                                                                  $"Step     : {d.Step}");
        }

        private void WriteColumns(string first, string second, InfoType firstInfoType = InfoType.Normal, InfoType secondInfoType = InfoType.Normal)
        {
            Write($"{first,-40}", firstInfoType);
            Console.Write(" ");
            Write(second, secondInfoType);
            Console.Write(Environment.NewLine);
        }

        private void WriteColumns(string first, string second, string third, InfoType firstInfoType = InfoType.Normal, InfoType secondInfoType = InfoType.Normal, InfoType thirdInfoType = InfoType.Normal)
        {
            Write($"{first,-40}", firstInfoType);
            Console.Write(" ");
            Write($"{second,-35}", secondInfoType);
            Console.Write(" ");
            Write($"{third,-20}", thirdInfoType);
                
            Console.Write(Environment.NewLine);
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
            Console.Write(text);
            Console.ForegroundColor = color;
        }
    }
}