namespace EtAlii.BinanceMagic
{
    using System;
    using System.Threading;
    using EtAlii.BinanceMagic.Surfing;

    public partial class Client 
    {
        public bool TryGetTrends(string[] coin, string referenceCoin, CancellationToken cancellationToken, out Trend[] trends, out string error)
        {
            trends = Array.Empty<Trend>();
            error = null;
            return true;
        }
    }
}