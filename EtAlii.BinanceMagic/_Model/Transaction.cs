namespace EtAlii.BinanceMagic
{
    using System;
    using System.IO;

    public record Transaction
    {
        public DateTime Moment { get; init; }
        public CoinSnapshot From { get; init; }
        public CoinSnapshot To { get; init; }
        
        public decimal Change { get; init; }
        public decimal TotalProfit { get; init; }

        public static void Write(StreamWriter sw, Transaction transaction)
        {
            sw.Write(transaction.Moment);
            sw.Write("|");
            CoinSnapshot.Write(sw, transaction.From);
            sw.Write("|");
            CoinSnapshot.Write(sw, transaction.To);
            sw.Write("|");
            sw.Write(transaction.Change);
            sw.Write("|");
            sw.Write(transaction.TotalProfit);
            sw.Write(Environment.NewLine);
        }
        
        public static Transaction Read(string line)
        {
            var parts = line.Split('|');

            return new Transaction
            {
                Moment = DateTime.Parse(parts[0]),
                From = CoinSnapshot.Read(parts[1]),
                To = CoinSnapshot.Read(parts[2]),
                Change = decimal.Parse(parts[3]),
                TotalProfit = decimal.Parse(parts[4])
            };
        }
    }
}