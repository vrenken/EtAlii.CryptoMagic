namespace EtAlii.BinanceMagic.Surfing
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class Data 
    {        
        private readonly IClient _client;
        private readonly AlgorithmSettings _settings;
        private readonly IOutput _output;
        public IReadOnlyList<Transaction> Transactions { get; } 
        private readonly List<Transaction> _transactions;

        private readonly string _transactionsFile;
        private FileStream _file;
        private StreamWriter _sw;

        public Data(IClient client, AlgorithmSettings settings, IOutput output)
        {
            _client = client;
            _settings = settings;
            _output = output;
            _transactions = new List<Transaction>();
            Transactions = _transactions.AsReadOnly();

            var coins = string.Join("_", _settings.AllowedCoins);
            _transactionsFile = string.Format(_settings.FileFormat, coins); 
        }

        public void Load()
        {
            _output.WriteLine("Loading previous transactions from file...");
            
            var lines = File.Exists(_transactionsFile) 
                ? File.ReadAllLines(_transactionsFile) 
                : Array.Empty<string>();
            var transactions = lines
                .Select(Transaction.Read)
                .ToArray();
            _transactions.AddRange(transactions);
            
            _file = new FileStream(_transactionsFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
            _sw = new StreamWriter(_file);

            _output.WriteLine("Loading previous transactions from file: Done");
        }
        
        public Coin FindLastPurchase(string coin) => _transactions.LastOrDefault(t => t.To.Symbol == coin)?.To;
        public Coin FindLastSell(string coin) => _transactions.LastOrDefault(t => t.To.Symbol == coin)?.From;

        public decimal GetTotalProfits()
        {
            return _transactions.Sum(t => t.Profit);
        }

        // public bool TryGetSituation(TradeDetails details, CancellationToken cancellationToken, out Situation situation)
        // {
        //     if (!_client.TryGetPrice(details.SellCoin, _settings.ReferenceCoin, details, cancellationToken, out var sourcePrice))
        //     {
        //         situation = null;
        //         return false;
        //     }
        //
        //     if (!_client.TryGetPrice(details.BuyCoin, _settings.ReferenceCoin, details, cancellationToken, out var targetPrice))
        //     {
        //         situation = null;
        //         return false;
        //     }
        //
        //     if (!_client.TryGetTradeFees(details.SellCoin, _settings.ReferenceCoin, details, cancellationToken, out var sourceMakerFee, out var _))
        //     {
        //         situation = null;
        //         return false;
        //     }
        //     
        //     if (!_client.TryGetTradeFees(details.BuyCoin, _settings.ReferenceCoin, details, cancellationToken, out var _, out var destinationTakerFee))
        //     {
        //         situation = null;
        //         return false;
        //     }
        //
        //     if (!_client.TryGetTrend(details.SellCoin, _settings.ReferenceCoin, details, cancellationToken, out var sellTrend))
        //     {
        //         situation = null;
        //         return false;
        //     }
        //     if (!_client.TryGetTrend(details.BuyCoin, _settings.ReferenceCoin, details, cancellationToken, out var buyTrend))
        //     {
        //         situation = null;
        //         return false;
        //     }
        //
        //
        //     var lastSourcePurchase = FindLastPurchase(details.SellCoin);
        //     var sourceDelta = new Delta
        //     {
        //         Coin = details.SellCoin,
        //         PastPrice = lastSourcePurchase?.Price ?? sourcePrice,
        //         PastQuantity = lastSourcePurchase?.Quantity ?? 0,
        //         PresentPrice = sourcePrice,
        //     };
        //
        //     var lastTargetSell = FindLastSell(details.BuyCoin);
        //     var targetDelta = new Delta
        //     {
        //         Coin = details.BuyCoin,
        //         PastPrice = lastTargetSell?.Price ?? targetPrice,
        //         PastQuantity = lastTargetSell?.Quantity ?? 0,
        //         PresentPrice = targetPrice
        //     };
        //     situation = new Situation
        //     {
        //         Source = sourceDelta,
        //         SellFee = sourceMakerFee,
        //         SellTrend = sellTrend,
        //         Destination = targetDelta,
        //         BuyFee = destinationTakerFee,
        //         BuyTrend = buyTrend,
        //         IsInitialCycle = lastSourcePurchase == null || lastTargetSell == null 
        //     };
        //     
        //     return true;
        // }

        public void AddTransaction(Transaction transaction)
        {
            _transactions.Add(transaction);
            Transaction.Write(_sw, transaction);
        }
    }
}