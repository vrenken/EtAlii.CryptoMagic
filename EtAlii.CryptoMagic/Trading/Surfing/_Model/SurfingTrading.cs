namespace EtAlii.CryptoMagic.Surfing
{
    using System.Collections.Generic;

    public class SurfingTrading : TradingBase
    {
        public IList<SurfingTransaction> Transactions { get; private set; } = new List<SurfingTransaction>();
    }
}