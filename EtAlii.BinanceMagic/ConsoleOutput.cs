namespace EtAlii.BinanceMagic
{
    using System;

    public static class ConsoleOutput
    {
        public static void Write(string line) => Console.WriteLine(line);
        public static void WritePositive(string line)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(line);
            Console.ForegroundColor = color;
        }
        public static void WriteNegative(string line)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(line);
            Console.ForegroundColor = color;
        }
    }
}