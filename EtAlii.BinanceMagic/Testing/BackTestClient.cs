namespace EtAlii.BinanceMagic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Binance.Net.Objects.Spot.MarketData;

    public class BackTestClient : IClient
    {
        private readonly IOutput _output;
        private readonly IProgram _program;
        private readonly string[] _coins;
        private readonly string _referenceCoin;
        private readonly Dictionary<string, HistoryEntry[]> _history = new();

        public TimeSpan Interval { get; } = TimeSpan.FromMinutes(1);

        public DateTime FirstRecordedHistory { get; private set; }
        public DateTime LastRecordedHistory { get; private set; }
        public DateTime Moment { get; set; }
        
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

            var lengths = _history
                .Select(h => h.Value.Length)
                .ToArray();

            var allSameLength = lengths.All(l => l == lengths[0]);
            if (!allSameLength)
            {
                _program.HandleFail("History files have different lengths");
            }

            var startTimes = _history.Select(h => h.Value
                    .Select(e => e.To)
                    .OrderBy(moment => moment)
                    .First())
                .ToArray();
            var allSameStartTime = startTimes.All(time => time == startTimes[0]);
            if(!allSameStartTime)            
            {
                _program.HandleFail("History files have different start times");
            }

            var endTimes = _history.Select(h => h.Value
                    .Select(e => e.To)
                    .OrderByDescending(moment => moment)
                    .First())
                .ToArray();
            var allSameEndTime = endTimes.All(time => time == endTimes[0]);
            if(!allSameEndTime)            
            {
                _program.HandleFail("History files have different end times");
            }

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

            var moment = DateTime.Parse(items[1]);
            return new HistoryEntry
            {
                To = moment,
                From = moment - Interval,

                Open = decimal.Parse(items[3]),
                High = decimal.Parse(items[4]),
                Low = decimal.Parse(items[5]),
                Close = decimal.Parse(items[6]),
            };
        }
        
        public void Stop() { }
        
        public BackTestClient(string[] coins, string referenceCoin, IOutput output, IProgram program)
        {
            _coins = coins;
            _referenceCoin = referenceCoin;
            _output = output;
            _program = program;
        }
        
        public bool TryGetPrice(string coin, string referenceCoin, TradeDetails details, CancellationToken cancellationToken, out decimal price)
        {
            var history = _history[coin].Single(entry => entry.From <= Moment && Moment <= entry.To);
            
            price = (history.Close + history.Open) / 2m;
            return true;
        }

        
        public bool TryConvert(SellAction sellAction, BuyAction buyAction, string referenceCoin, TradeDetails details, CancellationToken cancellationToken, Func<DateTime> getNow, out Transaction transaction)
        {
            transaction = new Transaction
            {
                Moment = getNow(),
                From = new Coin
                {

                },
                To = new Coin
                {

                }
            };
            
            return true;
        }

        public bool TryGetTradeFees(string coin, string referenceCoin, TradeDetails details, CancellationToken cancellationToken, out decimal makerFee, out decimal takerFee)
        {
            makerFee = 0.1m;
            takerFee = 0.1m;
            return true;
        }

        public bool TryGetTrend(string coin, string referenceCoin, TradeDetails details, CancellationToken cancellationToken, out decimal trend)
        {
            trend = 0m;            
            return true;
        }

        public bool TryGetExchangeInfo(TradeDetails details, CancellationToken cancellationToken, out BinanceExchangeInfo exchangeInfo)
        {
            exchangeInfo = null;
            return true;
        }
    }
}