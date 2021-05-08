namespace EtAlii.BinanceMagic.Surfing
{
    using System;
    using System.Threading.Tasks;

    public class StatusProvider : IStatusProvider
    {
        private readonly IOutput _output;
        private readonly TradeDetails _details;
        public const int CoinColumnWidth = 17;
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
            var status = _details.Status ?? "Waiting";
            
            _output.WriteLine("");
            WriteCoins();
            WriteVolumes();
            WritePrices();
            WriteTrends();
            WriteColumns($"Last success  : {lastSuccess}", $"Last profit   : +{_details.LastProfit:000.000000000} {_details.PayoutCoin}");
            WriteColumns($"Next check    : {nextCheck}",   $"Total profit  : +{_details.LastProfit:000.000000000} {_details.PayoutCoin}");
            _output.WriteLine($"Step          : {_details.Step}");
            
            _output.WriteLine("");
            _output.WriteLine($"Status        : {status}");
            
            Task.Delay(TimeSpan.FromSeconds(2)).Wait();
        }

        private void WriteCoins()
        {
            string line;
            ConsoleColor color;
            
            _output.Write("Current       :");
            
            foreach (var trend in _details.Trends)
            {
                line = CenteredString(trend.Coin, CoinColumnWidth);
                color = Console.ForegroundColor;
                Console.ForegroundColor = _details.CurrentCoin == trend.Coin
                    ? ConsoleColor.Yellow
                    : color;
                
                _output.Write(line);
                Console.ForegroundColor = color;
                _output.Write("|");
            }
            
            line = CenteredString(_details.PayoutCoin, CoinColumnWidth);
            color = Console.ForegroundColor;
            Console.ForegroundColor = _details.CurrentCoin == _details.PayoutCoin
                ? ConsoleColor.Yellow
                : color;

            _output.WriteLine(line);
            Console.ForegroundColor = color;
        }

        private void WriteVolumes()
        {
            string line;
            string volumeAsText;
            _output.Write("Volume        :");
            
            foreach (var trend in _details.Trends)
            {
                volumeAsText = trend.Coin == _details.CurrentCoin
                    ? $"{_details.CurrentVolume:000.000000000}"
                    : "";
                line = CenteredString(volumeAsText, CoinColumnWidth);
                _output.Write(line);
                _output.Write("|");
            }
            
            volumeAsText = _details.CurrentCoin == _details.PayoutCoin
                ? $"{_details.CurrentVolume:000.000000000}"
                : "";
            line = CenteredString(volumeAsText, CoinColumnWidth);
            _output.WriteLine(line);
        }

        private void WriteTrends()
        {
            _output.Write("Trends        :");
            
            foreach (var trend in _details.Trends)
            {
                var trendPrefix = trend.Change > 0 ? "+" : "";
                trendPrefix = trend.Change == 0m ? " " : trendPrefix;

                var trendAsText = $"{trendPrefix}{trend.Change}";
                var line = CenteredString(trendAsText, CoinColumnWidth);
                var color = Console.ForegroundColor;
                Console.ForegroundColor = trend.Change > 0 
                    ? ConsoleColor.Green
                    : ConsoleColor.Red;

                _output.Write(line);
                Console.ForegroundColor = color;
                _output.Write("|");
            }
            
            _output.WriteLine("");
        }

        
        private void WritePrices()
        {
            _output.Write("Price         :");
            
            foreach (var trend in _details.Trends)
            {
                var valueAsText = $"{trend.Price:000.000000000}";
                var line = CenteredString(valueAsText, CoinColumnWidth);
                _output.Write(line);
                _output.Write("|");
            }
            _output.WriteLine("");
        }

        private void WriteColumns(string first, string second, InfoType firstInfoType = InfoType.Normal, InfoType secondInfoType = InfoType.Normal)
        {
            Write($"{first,-40}", firstInfoType);
            _output.Write(" ");
            Write(second, secondInfoType);
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
        
        private string CenteredString(string s, int width)
        {
            if (s.Length >= width)
            {
                return s;
            }

            var leftPadding = (width - s.Length) / 2;
            var rightPadding = width - s.Length - leftPadding;

            return new string(' ', leftPadding) + s + new string(' ', rightPadding);
        }
    }

}