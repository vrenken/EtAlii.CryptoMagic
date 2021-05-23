namespace EtAlii.BinanceMagic.Service
{
    public record Delta
    {
        public string Symbol { get; init; }
        public decimal PastPrice { get; init; }
        public decimal PastQuantity { get; init; }
        
        public decimal PresentPrice { get; init; }
    }
}