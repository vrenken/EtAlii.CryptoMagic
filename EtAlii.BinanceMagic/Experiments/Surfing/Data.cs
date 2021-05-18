namespace EtAlii.BinanceMagic.Surfing
{
    using System.Collections.Generic;
    using System.Threading;

    public class Data 
    {        
        private readonly IClient _client;
        private readonly AlgorithmSettings _settings;
        private readonly IPersistence<Transaction> _persistence;
        public IReadOnlyList<Transaction> Transactions => _persistence.Items; 

        public Data(IClient client, AlgorithmSettings settings, IPersistence<Transaction> persistence)
        {
            _client = client;
            _settings = settings;
            _persistence = persistence;
        }

        public void Load() => _persistence.Load();
        public void AddTransaction(Transaction transaction) => _persistence.Add(transaction);

        public bool TryGetSituation(CancellationToken cancellationToken, TradeDetails details, out Situation situation, out string error)
        {
            if (!_client.TryGetTrends(_settings.AllowedCoins, _settings.PayoutCoin, _settings.RsiPeriod, cancellationToken, out var trends, out error))
            {
                situation = null;
                return false;
            }

            situation = new Situation
            {
                CurrentCoin = details.CurrentSymbol,
                Trends = trends
            };
            return true;
        }
    }
}