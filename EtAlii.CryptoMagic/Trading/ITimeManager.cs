namespace EtAlii.CryptoMagic
{
    using System;
    using System.Threading;

    public interface ITimeManager
    {
        bool ShouldStop();
        DateTime GetNow();
        void Wait(TimeSpan timeSpan, CancellationToken cancellationToken);
    }
}