namespace EtAlii.BinanceMagic
{
    using System;

    public class ConsoleOutput : IOutput
    {
        public void Write(string text) => Console.Write(text);
        
        public void WriteLine(string line) => Console.WriteLine(line);
        public void WriteLineFormatted(string line, params object[] parameters)
        {
            line = string.Format(line, parameters);
            Console.WriteLine(line);
        }

        public void WriteLinePositive(string line)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(line);
            Console.ForegroundColor = color;
        }
        public void WriteLineNegative(string line)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(line);
            Console.ForegroundColor = color;
        }
    }
}