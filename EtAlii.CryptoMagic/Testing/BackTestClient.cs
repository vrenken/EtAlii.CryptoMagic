namespace EtAlii.CryptoMagic
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Binance.Net.Objects.Models.Spot;
    using Serilog;

    public partial class BackTestClient : IClient
    {
        private readonly ILogger _log = Log.ForContext<BackTestClient>();
        private readonly IOutput _output;
        private readonly Guid _tradingId;
        private readonly string _folder;
        private readonly string[] _symbols;
        private readonly string _referenceSymbol;
        private readonly Dictionary<string, HistoryEntry[]> _history = new();

        private readonly Dictionary<string, HistoryEntry> _currentHistory = new();
        
        public TimeSpan Interval { get; } = TimeSpan.FromMinutes(1);

        public DateTime FirstRecordedHistory { get; private set; }
        public DateTime LastRecordedHistory { get; private set; }
        
        public DateTime Moment { get => _moment;
            set
            {
                var changed = _moment != value; 
                _moment = value;
                if (changed)
                {
                    SetCurrentHistory();
                }
            }
        }
        private DateTime _moment;

        public BackTestClient(string[] symbols, string referenceSymbol, IOutput output, Guid tradingId, string folder)
        {
            _symbols = symbols;
            _referenceSymbol = referenceSymbol;
            _output = output;
            _tradingId = tradingId;
            _folder = folder;
        }

        public Task Start(string apiKey, string secretKey)
        {
            _output.WriteLine("Preparing back-testing...");

            var folder = Path.Combine(_folder, "Cache", _tradingId.ToString());
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            
            foreach (var symbol in _symbols)
            {
                var fileName = $"Binance_{symbol}{_referenceSymbol}_minute.csv";
                var fullFileName = Path.Combine(folder, fileName);
                if (!File.Exists(fullFileName))
                {
                    var url = $"https://www.cryptodatadownload.com/cdd/{fileName}";
                    _output.WriteLine($"Downloading {url}");
                    using var client = new WebClient();
                    client.DownloadFile(url, fullFileName);
                }
                _output.WriteLine($"Reading from file {fileName}");
                
                _output.WriteLine($"Splitting in lines");

                var lines = File.ReadAllLines(fullFileName);
                
                _output.WriteLine($"Converting to objects");

                _history[symbol] = lines
                    .Skip(2)
                    .Select(ToHistoryEntry)
                    .Reverse()
                    .ToArray();
            }

            var startTimes = _history.Select(h => h.Value
                    .Select(e => e.To)
                    .OrderBy(moment => moment)
                    .First())
                .ToArray();

            var endTimes = _history.Select(h => h.Value
                    .Select(e => e.To)
                    .OrderByDescending(moment => moment)
                    .First())
                .ToArray();

            _output.WriteLine("History found:");
            for (var i = 0; i < _history.Count; i++)
            {
                _output.WriteLine($"{_history.Keys.ToArray()[i]}: {startTimes[i]} - {endTimes[i]}");                
            }
            
            FirstRecordedHistory = startTimes[0];
            LastRecordedHistory = endTimes[0];
            Moment = startTimes[0];
            
            _output.WriteLine("Ready for back-testing");
            return Task.CompletedTask;
        }

        public SymbolDefinition[] GetSymbols(string referenceSymbol) => throw new NotSupportedException();
        
        private HistoryEntry ToHistoryEntry(string line)
        {
            var items = line
                .Split(',')
                .ToArray();

            var moment = DateTime.ParseExact(items[1], "yyyy-MM-dd HH:mm:ss", DateTimeFormatInfo.InvariantInfo);
            return new HistoryEntry
            {
                To = moment,
                From = moment - Interval,

                Open = decimal.Parse(items[3], CultureInfo.InvariantCulture),
                High = decimal.Parse(items[4], CultureInfo.InvariantCulture),
                Low = decimal.Parse(items[5], CultureInfo.InvariantCulture),
                Close = decimal.Parse(items[6], CultureInfo.InvariantCulture),
                Volume = decimal.Parse(items[7], CultureInfo.InvariantCulture),
            };
        }

        public void Stop()
        {
            // In case of the BackTestClient there is nothing to stop.  
        }
        
        private void SetCurrentHistory()
        {
            foreach (var history in _history)
            {
                _currentHistory[history.Key] = history.Value.SingleOrDefault(entry => entry.From < Moment && Moment <= entry.To);
            }
        }
        
        public Task<(bool success, decimal price, string error)> TryGetPrice(string symbol, string referenceSymbol, CancellationToken cancellationToken)
        {
            var history = _currentHistory[symbol];

            if (history == null)
            {
                _log.Error("No history");
                return Task.FromResult((false, 0m, "No history"));
            }
            
            var price = (history.Close + history.Open) / 2m;
            return Task.FromResult<(bool success, decimal price, string error)>((false, price, null));
        }

        
        public Task<(bool success, TradeTransaction transaction, string error)> TryConvert(SellAction sellAction, BuyAction buyAction, string referenceSymbol, CancellationToken cancellationToken, Func<DateTime> getNow)
        {
            var transaction = new TradeTransaction
            {
                Sell = new Symbol
                {
                    SymbolName = sellAction.Symbol,
                    QuoteQuantity = sellAction.QuotedQuantity,
                    Quantity = sellAction.Quantity

                },
                Buy = new Symbol
                {
                    SymbolName = buyAction.Symbol,
                    QuoteQuantity = buyAction.QuotedQuantity,
                    Quantity = buyAction.Quantity
                },
                Moment = getNow(),
                Profit = sellAction.QuotedQuantity - buyAction.QuotedQuantity 
            };
            return Task.FromResult<(bool success, TradeTransaction transaction, string error)>((true, transaction, null));
        }

        public Task<(bool success, Symbol symbolsSold, string error)> TrySell(SellAction sellAction, string referenceSymbol, CancellationToken cancellationToken, Func<DateTime> getNow)
        {
            throw new NotImplementedException();
        }

        public async Task<(bool success, TradeTransaction transaction, string error)> TryBuyTransaction(BuyAction buyAction, string referenceSymbol, CancellationToken cancellationToken, Func<DateTime> getNow)
        {
            var (success, symbolsBought, error) = await TryBuySymbol(buyAction, referenceSymbol, cancellationToken, getNow); 
            if (!success)
            {
                return (false, null, error);
            }
            var transaction = new TradeTransaction
            {
                Sell = new Symbol
                {
                    SymbolName = "",
                    QuoteQuantity = 0,
                    Quantity = 0

                },
                Buy = symbolsBought,
                Moment = getNow(),
                Profit = 0 - buyAction.QuotedQuantity 
            };
            return (true, transaction, null);
        }

        public Task<(bool success, Symbol symbolsBought, string error)> TryBuySymbol(BuyAction buyAction, string referenceSymbol, CancellationToken cancellationToken, Func<DateTime> getNow)
        {
            var symbolsBought = new Symbol
            {
                SymbolName = buyAction.Symbol,
                QuoteQuantity = buyAction.QuotedQuantity,
                Quantity = buyAction.Quantity
            };
            return Task.FromResult<(bool success, Symbol symbolsBought, string error)>((true, symbolsBought, null)) ;
        }

        public decimal GetMinimalQuantity(string coin, BinanceExchangeInfo exchangeInfo, string referenceCoin)
        {
            return 10m;
        }

        public Task<(bool success, decimal makerFee, decimal takerFee, string error)> TryGetTradeFees(string symbol, string referenceSymbol, CancellationToken cancellationToken)
        {
            return Task.FromResult<(bool success, decimal makerFee, decimal takerFee, string error)>((true, 0.1m, 0.1m, null));
        }

        public Task<(bool success, BinanceExchangeInfo exchangeInfo, string error)> TryGetExchangeInfo(CancellationToken cancellationToken)
        {
            return Task.FromResult<(bool success, BinanceExchangeInfo exchangeInfo, string error)>((true, null, null));
        }

        public Task<(bool success, string error)> TryHasSufficientQuota(string symbol, decimal minimumValue)
        {
            return Task.FromResult<(bool success, string error)>((true, null));
        }
    }
}