namespace EtAlii.BinanceMagic.Service.Surfing
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class Data 
    {        
        private readonly IClient _client;
        private readonly AlgorithmSettings _settings;
        private readonly IPersistence<TradeTransaction> _persistence;
        public IReadOnlyList<TradeTransaction> Transactions => _persistence.Items; 

        public Data(IClient client, AlgorithmSettings settings, IPersistence<TradeTransaction> persistence)
        {
            _client = client;
            _settings = settings;
            _persistence = persistence;
        }

        public void Load() => _persistence.Load();
        public void AddTransaction(TradeTransaction transaction) => _persistence.Add(transaction);

        public async Task<(bool success, Situation situation, string error)> TryGetSituation(CancellationToken cancellationToken, TradeDetails details)
        {
            var (success, trends, error) = await _client.TryGetTrends(_settings.AllowedCoins, _settings.PayoutCoin, _settings.RsiPeriod, cancellationToken);
            if (!success)
            {
                return (false, null, error);
            }

            var situation = new Situation
            {
                CurrentCoin = details.CurrentSymbol,
                Trends = trends
            };
            return (true, situation, null);
        }
    }
}