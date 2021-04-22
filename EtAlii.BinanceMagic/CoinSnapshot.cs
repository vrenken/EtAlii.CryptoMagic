namespace EtAlii.BinanceMagic
{
    using System;
    using System.IO;

    public class CoinSnapshot
    {
        public string Coin { get; init; }
        public decimal Quantity { get; init; }
        public decimal Price { get; init; }

        public static void Write(StringWriter sw, CoinSnapshot coinSnapshot)
        {
            sw.Write(coinSnapshot.Coin);
            sw.Write("=");
            sw.Write(coinSnapshot.Quantity);
            sw.Write("=");
            sw.Write(coinSnapshot.Price);
            sw.Write(Environment.NewLine);
        }

        public static CoinSnapshot Read(string text)
        {
            var parts = text.Split("=");
            return new CoinSnapshot
            {
                Coin = parts[0],
                Quantity = decimal.Parse(parts[1]),
                Price = decimal.Parse(parts[2]),
            };
        }
    }
}