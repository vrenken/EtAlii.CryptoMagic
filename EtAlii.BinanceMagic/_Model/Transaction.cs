namespace EtAlii.BinanceMagic
{
    using System;
    using System.IO;

    public record Transaction
    {
        public DateTime Moment { get; init; }
        public Coin From { get; init; }
        public Coin To { get; init; }
        
        public decimal Target { get; init; }
        public decimal Profit { get; init; }

        public static void Write(StreamWriter sw, Transaction transaction)
        {
            sw.Write(transaction.Moment);
            sw.Write("|");
            Coin.Write(sw, transaction.From);
            sw.Write("|");
            Coin.Write(sw, transaction.To);
            sw.Write("|");
            sw.Write(transaction.Target);
            sw.Write("|");
            sw.Write(transaction.Profit);
            sw.Write(Environment.NewLine);
        }
        
        public static Transaction Read(string line)
        {
            var parts = line.Split('|');

            return new Transaction
            {
                Moment = DateTime.Parse(parts[0]),
                From = Coin.Read(parts[1]),
                To = Coin.Read(parts[2]),
                Target = decimal.Parse(parts[3]),
                Profit = decimal.Parse(parts[4])
            };
        }
    }
}