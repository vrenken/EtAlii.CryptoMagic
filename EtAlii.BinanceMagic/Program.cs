namespace EtAlii.BinanceMagic
{
    using System;

    public static class Program
    {
        static void Main()//string[] args)
        {
            var settings = new Settings();

            ConsoleOutput.Write("Starting Binance magic...");
            var loop = new AutomationLoop(settings);
            loop.Start();

            Console.ReadLine();
            ConsoleOutput.Write("Stopping Binance magic...");
            loop.Stop();
            ConsoleOutput.Write("Stopping Binance magic: Done");
        }
    }
}