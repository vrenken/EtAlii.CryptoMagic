namespace EtAlii.BinanceMagic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;

    public class Data : IData
    {        
        private readonly IClient _client;
        private readonly LoopSettings _settings;
        public IReadOnlyList<Transaction> Transactions { get; } 
        private readonly List<Transaction> _transactions;

        private readonly string _trendsFile;
        private readonly string _transactionsFile;
        public Data(IClient client, LoopSettings settings)
        {
            _client = client;
            _settings = settings;
            _transactions = new List<Transaction>();
            Transactions = _transactions.AsReadOnly();

            _trendsFile = string.Format(_settings.TrendsFileFormat, _settings.AllowedCoins[0], _settings.AllowedCoins[1]); 
            _transactionsFile = string.Format(_settings.TransactionsFileFormat, _settings.AllowedCoins[0], _settings.AllowedCoins[1]); 
        }

        public void Load()
        {
            ConsoleOutput.Write("Loading previous transactions from file...");
            
            var lines = File.Exists(_transactionsFile) 
                ? File.ReadAllLines(_transactionsFile) 
                : Array.Empty<string>();
            var transactions = lines
                .Select(Transaction.Read)
                .ToArray();
            _transactions.AddRange(transactions);
            ConsoleOutput.Write("Loading previous transactions from file: Done");
        }
        
        public CoinSnapshot FindLastPurchase(string coin) => _transactions.LastOrDefault(t => t.To.Coin == coin)?.To;
        public CoinSnapshot FindLastSell(string coin) => _transactions.LastOrDefault(t => t.To.Coin == coin)?.From;
        
        public bool TryGetSituation(TradeDetails details, CancellationToken cancellationToken, out Situation situation)
        {
            if (!_client.TryGetPrice(details.FromCoin, _settings.ReferenceCoin, details, cancellationToken, out var sourcePrice))
            {
                situation = null;
                return false;
            }

            if (!_client.TryGetPrice(details.ToCoin, _settings.ReferenceCoin, details, cancellationToken, out var targetPrice))
            {
                situation = null;
                return false;
            }

            if (!_client.TryGetTradeFees(details.FromCoin, _settings.ReferenceCoin, details, cancellationToken, out var sourceMakerFee, out var _))
            {
                situation = null;
                return false;
            }
            
            if (!_client.TryGetTradeFees(details.ToCoin, _settings.ReferenceCoin, details, cancellationToken, out var _, out var destinationTakerFee))
            {
                situation = null;
                return false;
            }

            var lastSourcePurchase = FindLastPurchase(details.FromCoin);
            var sourceDelta = new Delta
            {
                Coin = details.FromCoin,
                PastPrice = lastSourcePurchase?.Price ?? sourcePrice,
                PastQuantity = lastSourcePurchase?.Quantity ?? 0,
                PresentPrice = sourcePrice,
            };

            var lastTargetSell = FindLastSell(details.ToCoin);
            var targetDelta = new Delta
            {
                Coin = details.ToCoin,
                PastPrice = lastTargetSell?.Price ?? targetPrice,
                PastQuantity = lastTargetSell?.Quantity ?? 0,
                PresentPrice = targetPrice
            };
            situation = new Situation
            {
                Source =sourceDelta,
                SourceSellFee = sourceMakerFee,
                Destination = targetDelta,
                DestinationBuyFee = destinationTakerFee,
                IsInitialCycle = lastSourcePurchase == null || lastTargetSell == null 
            };
            
            return true;
        }

        public void AddTransaction(Transaction transaction)
        {
            _transactions.Add(transaction);
            using var file = new FileStream(_transactionsFile, FileMode.Append, FileAccess.Write, FileShare.Read);
            using var sw = new StreamWriter(file);

            Transaction.Write(sw, transaction);
        }

        public void AddTrend(decimal target, decimal sellPrice, decimal sellQuantity, decimal buyPrice, decimal buyQuantity, decimal difference)
        {
            var writeHeader = !File.Exists(_trendsFile); 
            using var file = new FileStream(_trendsFile, FileMode.Append, FileAccess.Write, FileShare.Read);
            using var sw = new StreamWriter(file);

            if (writeHeader)
            {
                sw.WriteLine($"Target;Sell price;Sell quantity;Buy price;Buy quantity;Difference");
            }
            sw.WriteLine($"{target};{sellPrice};{sellQuantity};{buyPrice};{buyQuantity};{difference}");
        }
    }
}