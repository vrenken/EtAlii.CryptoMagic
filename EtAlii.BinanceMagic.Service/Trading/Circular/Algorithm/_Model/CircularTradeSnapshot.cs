namespace EtAlii.BinanceMagic.Service
{
    using System;

    public class CircularTradeSnapshot : Entity
    {
        public CircularTrading Trading { get; init; }
        
        public string ReferenceSymbol { get; set; }
        
        public string SellSymbol { get; set; }
        public decimal SellPrice { get; set; }
        public decimal SellQuantity { get; set; }
        public decimal SellQuotedQuantity { get; set; }
        public decimal SellQuantityMinimum { get; set; }
        public decimal SellTrend { get; set; }

        public bool SellPriceIsAboveNotionalMinimum { get; set; }

        public string BuySymbol { get; set; }
        public decimal BuyPrice { get; set; }
        public decimal BuyQuantity { get; set; }
        public decimal BuyQuotedQuantity { get; set; }
        public decimal BuyQuantityMinimum { get; set; }
        public decimal BuyTrend { get; set; }
        public bool BuyPriceIsAboveNotionalMinimum { get; set; }
        
        public bool TrendsAreNegative { get; set; }
        public decimal Difference { get; set; }
        public decimal Target { get; set; }

        public int Step { get; set; }
        
        public bool SufficientProfit { get; set; }
        public DateTime LastSuccess { get; set; } = DateTime.MinValue;
        public DateTime LastCheck { get; set; }
        public DateTime NextCheck { get; set; }
        
        public decimal Profit { get; set; }

        public string Result { get; set; }

        public bool Error { get; set; }

        public bool IsWorthIt { get; set; }

        public CircularTradeSnapshot ShallowClone()
        {
            var other = (CircularTradeSnapshot) MemberwiseClone();
            other.Id = Guid.Empty;
            return other;
        }
    }
}