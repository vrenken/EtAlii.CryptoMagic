namespace EtAlii.BinanceMagic
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    public class AlgorithmContext<TSnapshot> : IAlgorithmContext<TSnapshot>, IDisposable
        where TSnapshot: class
    {
        public TSnapshot Snapshot
        {
            get => _snapshot;
            set
            {
                if (_snapshot != value)
                {
                    _snapshot = value;
                    RaiseChanged();
                }
            }
        }

        private TSnapshot _snapshot;
        
        private readonly Subject<object> _observable;
        private readonly IDisposable _subscription;

        public event Action<StatusInfo> Changed;

        public AlgorithmContext(TimeSpan? throttle = null)
        {
            if (throttle != null)
            {
                _observable = new Subject<object>();
                _subscription = _observable
                    .Sample(throttle.Value)
                    .Subscribe(RaiseChangedInternal);
            }

        }
        public void RaiseChanged() => RaiseChangedInternal(StatusInfo.Normal);
        public void RaiseChanged(StatusInfo statusInfo) => RaiseChangedInternal(statusInfo);

        private void RaiseChangedInternal(StatusInfo statusInfo)
        {
            if (statusInfo == StatusInfo.Important || _observable == null)
            {
                Changed?.Invoke(statusInfo);
            }
            else
            {
                _observable.OnNext(statusInfo);
            }
        }
        private void RaiseChangedInternal(object o)
        {
            var statusInfo = (StatusInfo) o;
            Changed?.Invoke(statusInfo);
        }

        public void Dispose()
        {
            _observable?.Dispose();
            _subscription?.Dispose();
        }
    }
}