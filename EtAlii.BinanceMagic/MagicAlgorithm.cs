namespace EtAlii.BinanceMagic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;

    public class MagicAlgorithm
    {
        private readonly Client _client;
        public const string ProfitFile = "Profits.txt";
        public const string GambleFile = "Gambles.txt";
        
        public static readonly string[] AllowedCoins = { "BTC", "BNB" }; // "ETH"

        public const string ApiKey = "tLLXzKjw2rmhbJeGZlGSEwWUzrKesTzlPNZphZLueMaaPzzaO7A7LYEszaC6QE4f";
        public const string SecretKey = "10Mr5QAxuEAcXGdtl10pKqHBx5HrsJcd5fdNbXN08gpjL8tFh7Vml2pEm2TVFtmd";

        public const string ReferenceCoin = "BUSD";
        
        public const decimal MinimalIncrease = 0.1m;

        public const decimal InitialPurchaseQuantity = 0.00000001m;

        public static readonly TimeSpan SampleInterval = TimeSpan.FromMinutes(5);

        public IReadOnlyList<Transaction> Transactions { get; }
        private readonly List<Transaction> _transactions;

        public MagicAlgorithm(Client client)
        {
            _client = client;
            _transactions = new();
            Transactions = _transactions.AsReadOnly();
        }
        public bool TransactionIsWorthIt(Target target, Situation situation)
        {
            return false;
        }

        public void Load()
        {
            ConsoleOutput.Write("Loading previous gambles from file...");
            
            var lines = File.Exists(MagicAlgorithm.GambleFile) 
                ? File.ReadAllLines(MagicAlgorithm.GambleFile) 
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
            var sourcePrice = _client.GetPrice(target.SourceCoin, cancellationToken);
            var targetPrice = _client.GetPrice(target.TargetCoin, cancellationToken);

            var sourceTradeFees = _client.GetTradeFees(target.SourceCoin, cancellationToken);
            var targetTradeFees = _client.GetTradeFees(target.TargetCoin, cancellationToken);

            var lastSourcePurchase = FindLastPurchase(target.SourceCoin);
            var sourceDelta = new Delta
            {
                Coin = target.SourceCoin,
                PastPrice = lastSourcePurchase?.Price ?? sourcePrice,
                PastQuota = lastSourcePurchase?.Quantity ?? 0,
                PresentPrice = sourcePrice,
            };

            var lastTargetSell = FindLastSell(target.TargetCoin);
            var targetDelta = new Delta
            {
                Coin = target.TargetCoin,
                PastPrice = lastTargetSell?.Price ?? targetPrice,
                PastQuota = lastTargetSell?.Quantity ?? 0,
                PresentPrice = targetPrice
            };
            return new Situation
            {
                SourceDelta =sourceDelta,
                SourceSellFee = sourceTradeFees.MakerFee,
                TargetDelta = targetDelta,
                TargetBuyFee = targetTradeFees.TakerFee,
            };
        }
        
        public Target BuildTarget(CancellationToken cancellationToken)
        {
            var lastTransaction = _transactions.LastOrDefault();

            var sourceCoin = lastTransaction == null
                ? AllowedCoins.First()
                : lastTransaction.To.Coin;
            var targetCoin = lastTransaction == null
                ? AllowedCoins.Skip(1).First()
                : lastTransaction.From.Coin;

            var lastSourcePurchase = FindLastPurchase(sourceCoin);
            var sourcePrice = lastSourcePurchase?.Price ?? _client.GetPrice(sourceCoin, cancellationToken);
            var sourceQuantity = lastSourcePurchase?.Quantity ?? InitialPurchaseQuantity;
            var minimalRequiredWinnings = sourcePrice * sourceQuantity * MinimalIncrease;
            
            return new Target
            {
                SourceCoin = sourceCoin,
                TargetCoin = targetCoin,
                MinimalRequiredGain = minimalRequiredWinnings, 
            };
        }
    }
}