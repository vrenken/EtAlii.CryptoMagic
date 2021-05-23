namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Microsoft.EntityFrameworkCore;

    public class AlgorithmContext<TTransaction, TTrading> : IAlgorithmContext<TTransaction, TTrading>, IDisposable
        where TTransaction: TransactionBase
        where TTrading: TradingBase
    {
        public TTransaction CurrentTransaction => _transaction;
        private TTransaction _transaction;

        public TTrading Trading => _trading;
        private TTrading _trading;

        private readonly Subject<object> _observable;
        private readonly IDisposable _subscription;

        public event Action<AlgorithmChange> Changed;

        public AlgorithmContext(TTrading trading, TTransaction transaction = null, TimeSpan? throttle = null)
        {
            _trading = trading;
            _transaction = transaction;
            
            if (throttle != null)
            {
                _observable = new Subject<object>();
                _subscription = _observable
                    .Sample(throttle.Value)
                    .Subscribe(RaiseChangedInternal);
            }

        }

        public void Update(TTrading trading, TTransaction transaction)
        {
            var isNewTrading = trading.Id == Guid.Empty;
            var isNewTransaction = trading.Id == Guid.Empty;

            _trading = trading;
            _transaction = transaction;
            
            var data = new DataContext();
            data.Attach(trading);
            data.Attach(transaction);
            data.Entry(trading).State = isNewTrading ? EntityState.Added : EntityState.Modified;
            data.Entry(transaction).State = isNewTransaction ? EntityState.Added : EntityState.Modified;
            data.SaveChanges();

            RaiseChanged(isNewTrading || isNewTransaction ? AlgorithmChange.Important : AlgorithmChange.Normal);
        }

        public void Update(TTrading trading)
        {
            var isNew = trading.Id == Guid.Empty;
            
            var data = new DataContext();
            data.Attach(trading);
            data.Entry(trading).State = isNew ? EntityState.Added : EntityState.Modified;
            data.SaveChanges();

            RaiseChanged(isNew ? AlgorithmChange.Important : AlgorithmChange.Normal);
        }

        public void Update(TTransaction transaction)
        {
            var isNew = transaction.Id == Guid.Empty;
            
            var data = new DataContext();
            data.Attach(transaction);
            data.Entry(transaction).State = isNew ? EntityState.Added : EntityState.Modified;
            data.SaveChanges();

            RaiseChanged(isNew ? AlgorithmChange.Important : AlgorithmChange.Normal);
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