namespace EtAlii.BinanceMagic
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Binance.Net.Objects.Spot.MarketData;

    public partial class BackTestClient : IClient
    {
        private readonly IOutput _output;
        private readonly string[] _coins;
        private readonly string _referenceCoin;
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

        public BackTestClient(string[] coins, string referenceCoin, IOutput output)
        {
            _coins = coins;
            _referenceCoin = referenceCoin;
            _output = output;
        }

        public void Start()
        {
            _output.WriteLine("Starting back-testing client...");

            foreach (var coin in _coins)
            {
                var fileName = $"Binance_{coin}{_referenceCoin}_minute.csv";
                fileName = Path.Combine("Testing", "History", fileName);
                
                _output.WriteLine($"Loading {fileName}");

                _history[coin] = File
                    .ReadLines(fileName)
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

                Open = decimal.Parse(items[3]),
                High = decimal.Parse(items[4]),
                Low = decimal.Parse(items[5]),
                Close = decimal.Parse(items[6]),
                Volume = decimal.Parse(items[7]),
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
        
        public bool TryGetPrice(string coin, string referenceCoin, CancellationToken cancellationToken, out decimal price, out string error)
        {
            var history = _currentHistory[coin];

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

        
        public bool TryConvert(SellAction sellAction, BuyAction buyAction, string referenceCoin, CancellationToken cancellationToken, Func<DateTime> getNow, out Transaction transaction, out string error)
        {
            transaction = new Transaction
            {
                From = new Coin
                {
                    Symbol = sellAction.Coin,
                    QuoteQuantity = sellAction.Price,
                    Quantity = sellAction.Quantity

                },
                To = new Coin
                {
                    Symbol = buyAction.Coin,
                    QuoteQuantity = buyAction.Price,
                    Quantity = buyAction.Quantity
                },
                Moment = getNow(),
                Profit = sellAction.Price - buyAction.Price 
            };
            error = null;
            return true;
        }

        public bool TrySell(SellAction sellAction, string referenceCoin, CancellationToken cancellationToken, Func<DateTime> getNow,
            out Coin coinsSold, out string error)
        {
            throw new NotImplementedException();
        }

        public bool TryBuy(BuyAction buyAction, string referenceCoin, CancellationToken cancellationToken, Func<DateTime> getNow,
            out Coin coinsBought, out string error)
        {
            throw new NotImplementedException();
        }

        public decimal GetMinimalQuantity(string coin, BinanceExchangeInfo exchangeInfo, string referenceCoin)
        {
            return 10m;
        }

        public bool TryGetTradeFees(string coin, string referenceCoin, CancellationToken cancellationToken, out decimal makerFee, out decimal takerFee, out string error)
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