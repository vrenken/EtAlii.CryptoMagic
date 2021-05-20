namespace EtAlii.BinanceMagic
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using Binance.Net.Objects.Spot.MarketData;

    public partial class BackTestClient : IClient
    {
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

        public void Start(string apiKey, string secretKey)
        {
            _output.WriteLine("Starting back-testing client...");

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
            // var allSameStartTime = startTimes.All(time => time == startTimes[0]);
            // if(!allSameStartTime)            
            // {
            //     _program.HandleFail("History files have different start times");
            // }

            var endTimes = _history.Select(h => h.Value
                    .Select(e => e.To)
                    .OrderByDescending(moment => moment)
                    .First())
                .ToArray();
            // var allSameEndTime = endTimes.All(time => time == endTimes[0]);
            // if(!allSameEndTime)            
            // {
            //     _program.HandleFail("History files have different end times");
            // }

            FirstRecordedHistory = startTimes[0];
            LastRecordedHistory = endTimes[0];
            Moment = startTimes[0];
            
            _output.WriteLine("Starting back-testing: Done");
        }

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
                Volume = decimal.Parse(items[8], CultureInfo.InvariantCulture),
            };
        }
        
        public void Stop() { }
        
        private void SetCurrentHistory()
        {
            foreach (var history in _history)
            {
                _currentHistory[history.Key] = history.Value.SingleOrDefault(entry => entry.From < Moment && Moment <= entry.To);
            }
        }
        
        public bool TryGetPrice(string symbol, string referenceSymbol, CancellationToken cancellationToken, out decimal price, out string error)
        {
            var history = _currentHistory[symbol];

            if (history == null)
            {
                price = 0m;
                error = "No history";
                return false;
            }
            
            price = (history.Close + history.Open) / 2m;
            error = null;
            return true;
        }

        
        public bool TryConvert(SellAction sellAction, BuyAction buyAction, string referenceSymbol, CancellationToken cancellationToken, Func<DateTime> getNow, out Transaction transaction, out string error)
        {
            transaction = new Transaction
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
            error = null;
            return true;
        }

        public bool TrySell(SellAction sellAction, string referenceSymbol, CancellationToken cancellationToken, Func<DateTime> getNow,
            out Symbol symbolsSold, out string error)
        {
            throw new NotImplementedException();
        }

        public bool TryBuy(BuyAction buyAction, string referenceSymbol, CancellationToken cancellationToken, Func<DateTime> getNow,
            out Symbol symbolsBought, out string error)
        {
            throw new NotImplementedException();
        }

        public decimal GetMinimalQuantity(string coin, BinanceExchangeInfo exchangeInfo, string referenceCoin)
        {
            return 10m;
        }

        public bool TryGetTradeFees(string symbol, string referenceSymbol, CancellationToken cancellationToken, out decimal makerFee, out decimal takerFee, out string error)
        {
            makerFee = 0.1m;
            takerFee = 0.1m;
            error = null;
            return true;
        }

        public bool TryGetExchangeInfo(CancellationToken cancellationToken, out BinanceExchangeInfo exchangeInfo, out string error)
        {
            exchangeInfo = null;
            error = null;
            return true;
        }
    }
}