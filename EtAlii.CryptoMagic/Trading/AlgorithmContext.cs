namespace EtAlii.CryptoMagic
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Microsoft.EntityFrameworkCore;

    public class AlgorithmContext<TTransaction, TTrading> : IAlgorithmContext<TTransaction, TTrading>, IDisposable
        where TTransaction: TransactionBase<TTrading>
        where TTrading: TradingBase
    {
        public TTransaction CurrentTransaction { get; private set; }
        public TTrading Trading { get; }

        private readonly Subject<object> _observable;
        private readonly IDisposable _subscription;

        public event Action<AlgorithmChange> Changed;

        public AlgorithmContext(TTrading trading, TimeSpan? throttle = null)
        {
            Trading = trading;
            
            if (throttle != null)
            {
                _observable = new Subject<object>();
                _subscription = _observable
                    .Sample(throttle.Value)
                    .Subscribe(RaiseChangedInternal);
            }
        }

        public void Update(TTransaction transaction)
        {
            var isNewTrading = Trading.Id == Guid.Empty;
            var isNewTransaction = transaction.Id == Guid.Empty;
            var hasEndDate = Trading.End != null;
            
            var data = new DataContext();

            if (CurrentTransaction != null && CurrentTransaction != transaction)
            {
                CurrentTransaction.Trading = Trading;
                data.Entry(CurrentTransaction).State = EntityState.Modified;
            }
            
            transaction.Trading = Trading;

            data.Entry(Trading).State = isNewTrading ? EntityState.Added : EntityState.Modified;
            data.Entry(transaction).State = isNewTransaction ? EntityState.Added : EntityState.Modified;
            data.SaveChanges();

            CurrentTransaction = transaction;
            RaiseChanged(hasEndDate || isNewTrading || isNewTransaction ? AlgorithmChange.Important : AlgorithmChange.Normal);
        }

        private void RaiseChanged(AlgorithmChange algorithmChange = AlgorithmChange.Normal)
        {
            if (algorithmChange == AlgorithmChange.Important || _observable == null)
            {
                Changed?.Invoke(algorithmChange);
            }
            else
            {
                _observable.OnNext(algorithmChange);
            }
        }
        private void RaiseChangedInternal(object o)
        {
            var statusInfo = (AlgorithmChange) o;
            Changed?.Invoke(statusInfo);
        }

        public void Dispose()
        {
            _observable?.Dispose();
            _subscription?.Dispose();
        }
    }
}