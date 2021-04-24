namespace EtAlii.BinanceMagic
{
    public record Delta
    {
        public string Coin { get; init; }
        public decimal PastPrice { get; init; }
        public decimal PastQuantity { get; init; }
        
        public decimal PresentPrice { get; init; }
    }
}