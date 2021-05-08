namespace EtAlii.BinanceMagic.Surfing
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;

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

        public bool TryGetSituation(CancellationToken cancellationToken, TradeDetails details, out Situation situation)
        {
            if (!_client.TryGetTrends(_settings.AllowedCoins, _settings.PayoutCoin, cancellationToken, out var trends, out var error))
            {
                details.Status = error;
                situation = null;
                return false;
            }

            situation = new Situation
            {
                CurrentCoin = details.CurrentCoin,
                Trends = trends
            };
            return true;
        }

        public void AddTransaction(Transaction transaction)
        {
            _transactions.Add(transaction);
            Transaction.Write(_sw, transaction);
        }
    }
}