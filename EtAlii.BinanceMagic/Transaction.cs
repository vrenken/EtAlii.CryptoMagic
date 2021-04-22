namespace EtAlii.BinanceMagic
{
    using System;
    using System.IO;

    public class Transaction
    {
        public DateTime Moment { get; init; }
        public CoinSnapshot From { get; init; }
        public CoinSnapshot To { get; init; }

        public static void Write(StringWriter sw, Transaction transaction)
        {
            sw.Write(transaction.Moment);
            sw.Write("|");
            CoinSnapshot.Write(sw, transaction.From);
            sw.Write("|");
            CoinSnapshot.Write(sw, transaction.To);
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
            };
        }
    }
}