namespace EtAlii.BinanceMagic.Service
{
    using System;

    public class SimpleTransaction : TransactionBase
    {
        public decimal Sell { get; set; }
        public decimal SellQuantity { get; set; }
        public decimal SellTrend { get; set; }

        public decimal Buy { get; set; }
        public decimal BuyQuantity { get; set; }
        public decimal BuyTrend { get; set; }

        public decimal Diff { get; set; }


        public decimal Target { get; set; }

        public decimal Profit { get; set; }

        public DateTime LastSuccess { get; set; }
        public DateTime LastCheck { get; set; }
        public DateTime NextCheck { get; set; }
        public string Result { get; set; }
        public int Step { get; set; }
    }
}