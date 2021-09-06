namespace EtAlii.CryptoMagic.Surfing
{
    using System;
    using System.Threading.Tasks;

    public class AlgorithmContext : AlgorithmContext<SurfingTransaction, SurfingTrading>
    {
        private readonly IOutput _output;
        private readonly TradeDetails _details;
        private readonly AlgorithmSettings _settings;

        private const int SymbolColumnWidth = 17;
        
        public AlgorithmContext(IOutput output, TradeDetails details, AlgorithmSettings settings)
            : base(null)
        {
            _output = output;
            _details = details;
            _settings = settings;
        }
        
        public void Write()
        {
            var nextCheck = _details.NextCheck != DateTime.MinValue ? _details.NextCheck.ToString("yyyy-MM-dd HH:mm:ss") : "Now...";
            var lastSuccess = _details.LastSuccess != DateTime.MinValue ? _details.LastSuccess.ToString("yyyy-MM-dd HH:mm:ss") : "None";
            var status = _details.Status ?? "Waiting";

            var totalProfit = _details.TotalProfit;
            var totalProfitPrefix = totalProfit > 0 ? "+" : "";
            totalProfitPrefix = totalProfit == 0m ? " " : totalProfitPrefix;

            _output.WriteLine("");
            WriteCoins();
            WriteVolumes();
            WritePrices();
            WriteValues();
            WriteTrends();
            WriteColumns($"Last success    : {lastSuccess}", $"Last profit    : +{_details.LastProfit:000.000000000} {_details.PayoutSymbol}");
            WriteColumns($"Next check      : {nextCheck}",   $"Total profit   : {totalProfitPrefix}{totalProfit:000.000000000} {_details.PayoutSymbol}");
            _output.WriteLine($"Step            : {_details.Step}");
            
            _output.WriteLine("");
            _output.WriteLine($"Status          : {status}");
            
            Task.Delay(TimeSpan.FromMilliseconds(10)).Wait();
        }

        private void WriteCoins()
        {
            string line;
            ConsoleColor color;
            
            _output.Write("Current         :");
            
            foreach (var trend in _details.Trends)
            {
                line = CenteredString(trend.Symbol, SymbolColumnWidth);
                color = Console.ForegroundColor;
                Console.ForegroundColor = _details.CurrentSymbol == trend.Symbol
                    ? ConsoleColor.Yellow
                    : color;
                
                _output.Write(line);
                Console.ForegroundColor = color;
                _output.Write("|");
            }
            
            line = CenteredString(_details.PayoutSymbol, SymbolColumnWidth);
            color = Console.ForegroundColor;
            Console.ForegroundColor = _details.CurrentSymbol == _details.PayoutSymbol
                ? ConsoleColor.Yellow
                : color;

            _output.WriteLine(line);
            Console.ForegroundColor = color;
        }

        private void WriteVolumes()
        {
            string line;
            string volumeAsText;
            _output.Write("Volume          :");
            
            foreach (var trend in _details.Trends)
            {
                volumeAsText = trend.Symbol == _details.CurrentSymbol
                    ? $"{_details.CurrentVolume:000.000000000}"
                    : "";
                line = CenteredString(volumeAsText, SymbolColumnWidth);
                _output.Write(line);
                _output.Write("|");
            }
            
            // volumeAsText = _details.CurrentCoin == _details.PayoutCoin
            //     ? $"{_details.CurrentVolume:000.000000000}"
            //     : "";
            // line = CenteredString(volumeAsText, CoinColumnWidth);
            // _output.WriteLine(line);
            _output.WriteLine("");
        }

        
        private void WriteValues()
        {
            string line;
            string valueAsText;
            _output.Write("Value           :");
            
            foreach (var trend in _details.Trends)
            {
                valueAsText = trend.Symbol == _details.CurrentSymbol
                    ? $"{_details.CurrentVolume * trend.Price:000.000000000}"
                    : "";
                line = CenteredString(valueAsText, SymbolColumnWidth);
                _output.Write(line);
                _output.Write("|");
            }
            //
            // valueAsText = _details.CurrentCoin == _details.PayoutCoin
            //     ? $"{_details.CurrentVolume:000.000000000}"
            //     : "";
            // line = CenteredString(valueAsText, CoinColumnWidth);
            // _output.WriteLine(line);
            
            valueAsText = _details.CurrentSymbol == _details.PayoutSymbol
                ? $"{_details.CurrentVolume:000.000000000}"
                : "";
            line = CenteredString(valueAsText, SymbolColumnWidth);
            _output.WriteLine(line);
        }

        private void WriteTrends()
        {
            _output.Write($"Trends (RSI{_settings.RsiPeriod:00})  :");
            
            foreach (var trend in _details.Trends)
            {
                // var trendPrefix = trend.Change > 0 ? "+" : "";
                // trendPrefix = trend.Change == 0m ? " " : trendPrefix;
                // var trendAsText = $"{trendPrefix}{trend.Change}";
                var trendAsText = $"{trend.Rsi * 100:0.00}";
                
                var line = CenteredString(trendAsText, SymbolColumnWidth);
                var color = Console.ForegroundColor;
                if (trend.Symbol == _details.CurrentSymbol)
                {
                    Console.ForegroundColor = trend.Rsi > _settings.RsiOverSold 
                        ? ConsoleColor.Green
                        : ConsoleColor.Red;
                }
                else
                {
                    Console.ForegroundColor = trend.Rsi > _settings.RsiOverBought 
                        ? ConsoleColor.Green
                        : ConsoleColor.Red;
                }

                _output.Write(line);
                Console.ForegroundColor = color;
                _output.Write("|");
            }
            
            _output.WriteLine("");
        }

        
        private void WritePrices()
        {
            _output.Write("Price           :");
            
            foreach (var trend in _details.Trends)
            {
                var valueAsText = $"{trend.Price:000.000000000}";
                var line = CenteredString(valueAsText, SymbolColumnWidth);
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