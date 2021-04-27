namespace EtAlii.BinanceMagic
{
    using System;

    public class StatusInfo
    {
        public string FromCoin { get; set; }
        public string ToCoin { get; set; }
        public string ReferenceCoin { get; set; }
        
        public decimal SellPrice { get; set; }
        public decimal SellQuantity { get; set; }
        public decimal SellQuantityMinimum { get; set; }
        public bool SellPriceIsAboveNotionalMinimum { get; set; }

        public decimal BuyPrice { get; set; }
        public decimal BuyQuantity { get; set; }
        public decimal BuyQuantityMinimum { get; set; }
        public bool BuyPriceIsAboveNotionalMinimum { get; set; }
        public decimal Difference { get; set; }
        public decimal Target { get; set; }

        public bool SufficientProfit { get; set; }
        public DateTime LastSuccess { get; set; } = DateTime.MinValue;
        public DateTime LastCheck { get; set; }
        public DateTime NextCheck { get; set; }
        
        public decimal Profit { get; set; }

        public string Result
        {
            get => _result;
            set
            {
                _result = value; 
                LastCheck = DateTime. Now;
                DumpToConsole();
            }
        }

        private string _result;
        public bool Error { get; set; }

        public bool IsWorthIt { get; set; }

        public void DumpToConsole()
        {
            var nextCheck = NextCheck != DateTime.MinValue ? NextCheck.ToString("yyyy-MM-dd HH:mm:ss") : "Now";
            var lastSuccess = LastSuccess != DateTime.MinValue ? LastSuccess.ToString("yyyy-MM-dd HH:mm:ss") : "None";

            var differencePrefix = Difference > 0 ? "+" : "";
            Console.Clear();
            Console.WriteLine();
            ConsoleOutput.Write($"{FromCoin}->{ToCoin}");
            WriteColumns($"Sell         : +{SellPrice:000.000000000} {ReferenceCoin}",                      $"Quantity : +{SellQuantity:000.000000000} {FromCoin}", SellPriceIsAboveNotionalMinimum ? InfoType.Positive : InfoType.Negative);
            WriteColumns($"Buy          : -{BuyPrice:000.000000000} {ReferenceCoin}",                       $"Quantity : -{BuyQuantity:000.000000000} {ToCoin}", BuyPriceIsAboveNotionalMinimum ? InfoType.Positive : InfoType.Negative);
            WriteColumns($"Diff         : {differencePrefix}{Difference:000.000000000} {ReferenceCoin}",    $"Target   : +{Target:000.000000000} {ReferenceCoin}", SufficientProfit ? InfoType.Positive : InfoType.Negative);
            WriteColumns($"Last success : {lastSuccess}",                                                             $"Profit   : +{Profit:000.000000000} {ReferenceCoin}");
            WriteColumns($"Last check   : {LastCheck:yyyy-MM-dd HH:mm:ss}",                                           $"Result   : {Result}");
            WriteColumns($"Next check   : {nextCheck}", $"");
        }

        private void WriteColumns(string first, string second, InfoType firstInfoType = InfoType.Normal, InfoType secondInfoType = InfoType.Normal)
        {
            Write($"{first,-40}", firstInfoType);
            Console.Write(" ");
            Write(second, secondInfoType);
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