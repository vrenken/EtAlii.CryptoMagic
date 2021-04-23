namespace EtAlii.BinanceMagic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;

    public class DataProvider
    {        
        private readonly Client _client;
        private readonly Settings _settings;
        public IReadOnlyList<Transaction> Transactions { get; } 
        private readonly List<Transaction> _transactions;

        public DataProvider(Client client, Settings settings)
        {
            _client = client;
            _settings = settings;
            _transactions = new();
            Transactions = _transactions.AsReadOnly();
        }

        public void Load()
        {
            ConsoleOutput.Write("Loading previous gambles from file...");
            
            var lines = File.Exists(_settings.TransactionsFile) 
                ? File.ReadAllLines(_settings.TransactionsFile) 
                : Array.Empty<string>();
            var transactions = lines
                .Select(Transaction.Read)
                .ToArray();
            _transactions.AddRange(transactions);
            ConsoleOutput.Write("Loading previous gambles from file: Done");
        }
        
        private CoinSnapshot FindLastPurchase(string coin) => _transactions.SingleOrDefault(t => t.To.Coin == coin)?.To;
        private CoinSnapshot FindLastSell(string coin) => _transactions.SingleOrDefault(t => t.To.Coin == coin)?.From;
        
        public Situation GetSituation(Target target, CancellationToken cancellationToken)
        {
            var sourcePrice = _client.GetPrice(target.Source, cancellationToken);
            var targetPrice = _client.GetPrice(target.Destination, cancellationToken);

            var sourceTradeFees = _client.GetTradeFees(target.Source, cancellationToken);
            var targetTradeFees = _client.GetTradeFees(target.Destination, cancellationToken);

            var lastSourcePurchase = FindLastPurchase(target.Source);
            var sourceDelta = new Delta
            {
                Coin = target.Source,
                PastPrice = lastSourcePurchase?.Price ?? sourcePrice,
                PastQuota = lastSourcePurchase?.Quantity ?? 0,
                PresentPrice = sourcePrice,
            };

            var lastTargetSell = FindLastSell(target.Destination);
            var targetDelta = new Delta
            {
                Coin = target.Destination,
                PastPrice = lastTargetSell?.Price ?? targetPrice,
                PastQuota = lastTargetSell?.Quantity ?? 0,
                PresentPrice = targetPrice
            };
            var situation = new Situation
            {
                SourceDelta =sourceDelta,
                SourceSellFee = sourceTradeFees.MakerFee,
                DestinationDelta = targetDelta,
                DestinationBuyFee = targetTradeFees.TakerFee,
                IsInitialCycle = lastSourcePurchase == null || lastTargetSell == null 
            };
            
            WriteSituation(situation, target);

            return situation;
        }

        private void WriteSituation(Situation situation, Target target)
        {
            var quotaLabel   = "- Quota   = ";
            var pastLabel    = "- Past    = ";
            var presentLabel = "- Present = ";
            var feeLabel     = "- Fee     = ";
            var format = "{0, -12}{1, -30}{2, -12}{3, -30}";
            ConsoleOutput.Write($"Current situation:");
            ConsoleOutput.Write(string.Format(format, situation.SourceDelta.Coin, "", situation.DestinationDelta.Coin, ""));
            ConsoleOutput.Write(string.Format(format, quotaLabel,$"{situation.SourceDelta.PastQuota} {_settings.ReferenceCoin}",quotaLabel, $"{situation.DestinationDelta.PastQuota} {_settings.ReferenceCoin}"));
            ConsoleOutput.Write(string.Format(format, pastLabel, $"{situation.SourceDelta.PastPrice * situation.SourceDelta.PastQuota}", pastLabel, $"{situation.DestinationDelta.PastPrice * situation.DestinationDelta.PastQuota}"));
            ConsoleOutput.Write(string.Format(format, presentLabel, $"{situation.SourceDelta.PresentPrice * situation.SourceDelta.PastQuota} {_settings.ReferenceCoin}", presentLabel, $"{situation.DestinationDelta.PresentPrice * situation.DestinationDelta.PastQuota} {_settings.ReferenceCoin}"));
            ConsoleOutput.Write(string.Format(format, feeLabel, $"{situation.SourceSellFee:P}", feeLabel, $"{situation.DestinationBuyFee:P}"));

            ConsoleOutput.Write($"Target  = {target.Profit} {_settings.ReferenceCoin}");
        }
        
        public Target BuildTarget(CancellationToken cancellationToken)
        {
            var lastTransaction = _transactions.LastOrDefault();

            var source = lastTransaction == null
                ? _settings.AllowedCoins.First()
                : lastTransaction.To.Coin;
            var destination = lastTransaction == null
                ? _settings.AllowedCoins.Skip(1).First()
                : lastTransaction.From.Coin;

            var profit = lastTransaction != null
                ? lastTransaction.TotalProfit * (1 + _settings.MinimalIncrease)
                : _settings.InitialTargetProfit;
             
            return new Target
            {
                Source = source,
                Destination = destination,
                Profit = profit,  
            };
        }
        
        public void AddTransaction(Transaction transaction)
        {
            _transactions.Add(transaction);
            using var file = new FileStream(_settings.TransactionsFile, FileMode.Append, FileAccess.Write, FileShare.Read);
            using var sw = new StreamWriter(file);

            Transaction.Write(sw, transaction);
        }
    }
}