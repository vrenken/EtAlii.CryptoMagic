namespace EtAlii.BinanceMagic
{
    using System;

    public class Program
    {
        private readonly Settings _settings;

        public Program(Settings settings)
        {
            _settings = settings;
        }
        static void Main()//string[] args)
        {
            var settings = new Settings();
            var program = new Program(settings);

            ConsoleOutput.Write("Starting Binance magic...");
            var loop = new Loop(settings, program);
            loop.Start();

            Console.ReadLine();
            ConsoleOutput.Write("Stopping Binance magic...");
            loop.Stop();
            ConsoleOutput.Write("Stopping Binance magic: Done");
        }
        
        public void HandleFail(string message)
        {
            if (_settings.IsTest)
            {
                throw new InvalidOperationException(message);
            }

            ConsoleOutput.WriteNegative(message);
            Environment.Exit(-1);
        }


    }
}