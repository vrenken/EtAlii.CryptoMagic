namespace EtAlii.BinanceMagic
{
    using System;
    using System.Threading;
    using EtAlii.BinanceMagic.Surfing;

    public partial class BackTestClient 
    {
        public bool TryGetTrends(string[] coin, string referenceCoin, int period, CancellationToken cancellationToken, out Trend[] trends, out string error)
        {
            throw new NotSupportedException();
        }
    }
}