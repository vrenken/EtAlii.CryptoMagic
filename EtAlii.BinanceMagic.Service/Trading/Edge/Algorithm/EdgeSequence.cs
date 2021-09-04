#nullable enable

namespace EtAlii.BinanceMagic.Service
{
    public partial class EdgeSequence 
    {
        // private readonly IClient _client;
        private readonly ITimeManager _timeManager;
        private readonly IAlgorithmContext<EdgeTransaction, EdgeTrading> _context;
        private readonly ITargetTransactionFinder _targetTransactionFinder;

        public EdgeSequence(ITimeManager timeManager, IAlgorithmContext<EdgeTransaction, EdgeTrading> context, ITargetTransactionFinder targetTransactionFinder)
        {
            _timeManager = timeManager;
            _context = context;
            _targetTransactionFinder = targetTransactionFinder;
        }
    }
}