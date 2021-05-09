namespace EtAlii.BinanceMagic
{
    using System.IO;

    public record Coin
    {
        public string Symbol { get; init; }
        public decimal Quantity { get; init; }
        public decimal Price { get; init; }
        public decimal QuoteQuantity { get; init; }

        public static void Write(StreamWriter sw, Coin coin)
        {
            sw.Write(coin.Symbol);
            sw.Write("=");
            sw.Write(coin.Quantity);
            sw.Write("=");
            sw.Write(coin.QuoteQuantity);
            sw.Write("=");
            sw.Write(coin.Price);
        }

        public static Coin Read(string text)
        {
            var parts = text.Split("=");
            return new Coin
            {
                Symbol = parts[0],
                Quantity = decimal.Parse(parts[1]),
                QuoteQuantity = decimal.Parse(parts[2]),
                Price = decimal.Parse(parts[3]),
            };
        }
    }
}