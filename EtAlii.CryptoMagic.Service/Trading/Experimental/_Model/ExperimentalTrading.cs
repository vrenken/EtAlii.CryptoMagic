namespace EtAlii.CryptoMagic.Service
{
    using System.Collections.Generic;

    public class ExperimentalTrading : TradingBase
    {
        public IList<ExperimentalTransaction> Transactions { get; private set; } = new List<ExperimentalTransaction>();
    }
}