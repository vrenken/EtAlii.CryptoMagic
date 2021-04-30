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
        private readonly string[] _coins;
        private readonly string _referenceCoin;
        private readonly Dictionary<string, HistoryEntry[]> _history = new();

        private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);
                
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
                From = moment - _interval,

                Open = decimal.Parse(items[3]),
                High = decimal.Parse(items[4]),
                Low = decimal.Parse(items[5]),
                Close = decimal.Parse(items[6]),
            };
        }
        
        public void Stop() { }
        
        public BackTestClient(string[] coins, string referenceCoin, IOutput output)
        {
            _coins = coins;
            _referenceCoin = referenceCoin;
            _output = output;
        }
        
        public bool TryGetPrice(string coin, string referenceCoin, TradeDetails details, CancellationToken cancellationToken, out decimal price)
        {
            var history = _history[coin].Single(entry => entry.From <= Moment && Moment <= entry.To);
            
            price = (history.Close + history.Open) / 2m;
            return true;
        }

        
        public bool TryConvert(SellAction sellAction, BuyAction buyAction, string referenceCoin, TradeDetails details, CancellationToken cancellationToken, out Transaction transaction)
        {

            transaction = new Transaction
            {
                Moment = Moment,
                From = new Coin
                {

                },
                To = new Coin
                {

                }
            };
            
            return true;
        }

        public bool TryGetTradeFees(string coin, string referenceCoin, TradeDetails details,
            CancellationToken cancellationToken, out decimal makerFee, out decimal takerFee)
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