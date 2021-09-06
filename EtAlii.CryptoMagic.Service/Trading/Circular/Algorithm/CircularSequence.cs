#nullable enable
#pragma warning disable SL2001

namespace EtAlii.CryptoMagic.Service
{
    public class CircularSequence : CircularSequenceBase
    {
        private readonly ITimeManager _timeManager;
        private readonly IAlgorithmContext<CircularTransaction, CircularTrading> _context;
        private readonly ITargetTransactionFinder _targetTransactionFinder;
        
        public CircularSequence(
            ITimeManager timeManager,
            IAlgorithmContext<CircularTransaction, CircularTrading> context)
        {
            _timeManager = timeManager;
            _context = context;
            _targetTransactionFinder = new TargetTransactionFinder(_context);
        }

        protected override void OnLoadPreviousCycleFromDatabaseEntered()
        {
            var transaction = _targetTransactionFinder.Find();
            transaction.LastCheck = _timeManager.GetNow();
            _context.Update(transaction);
        }

        protected override void OnCheckWhatCycleEntered(CheckWhatCycleEventArgs e)
        {
            var data = new DataContext();
            var cycle = data.GetCycle(_context.Trading);

            switch (cycle)
            {
                case Cycle.BuyA: e.IsInitialCycleToA(); break;
                case Cycle.SellABuyB: e.IsInitialCycleToB(); break;
                case Cycle.TransferFromAToB: e.IsNormalCycleFromAToB(); break;
                case Cycle.TransferFromBToA: e.IsNormalCycleFromBToA(); break;
            }
        }

        protected override void OnCheckIfSufficientReferenceInInitialPurchaseOfAEntered()
        {
            //_client.TryHasSufficientQuota(_context.Trading.ReferenceSymbol, )

            // var available = 0m;
            // var required = 0m;
            // Continue();
        }
    }
}