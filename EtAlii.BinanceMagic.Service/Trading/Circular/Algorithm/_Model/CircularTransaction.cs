namespace EtAlii.BinanceMagic.Service
{
    using System;

    public class CircularTransaction : TransactionBase<CircularTrading>
    {
        public string SellSymbol { get; set; }
        public decimal SellPrice { get; set; }
        public decimal SellQuantity { get; set; }
        public decimal SellQuotedQuantity { get; set; }
        public decimal SellQuantityMinimum { get; set; }
        public decimal SellTrend { get; set; }
        public bool SellTrendIsOptimal { get; set; }

        public bool SellPriceIsOptimal { get; set; }

        public string BuySymbol { get; set; }
        public decimal BuyPrice { get; set; }
        public decimal BuyQuantity { get; set; }
        public decimal BuyQuotedQuantity { get; set; }
        public decimal BuyQuantityMinimum { get; set; }
        public decimal BuyTrend { get; set; }
        public bool BuyTrendIsOptimal { get; set; }

        public bool BuyPriceIsOptimal { get; set; }
        
        public decimal Difference { get; set; }
        public decimal Target { get; set; }

        public int Step { get; set; }
        
        public bool DifferenceIsOptimal { get; set; }
        public DateTime? LastSuccess { get; set; }
        public DateTime? LastCheck { get; set; }
        public DateTime? NextCheck { get; set; }
        
        public decimal Profit { get; set; }

        public string Result { get; set; }

        public bool IsWorthIt { get; set; }
        public bool Completed { get; set; }

        public CircularTransaction ShallowClone()
        {
            var other = (CircularTransaction) MemberwiseClone();
            other.Id = Guid.Empty;
            other.Completed = false;
            return other;
        }
    }
}