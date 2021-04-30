namespace EtAlii.BinanceMagic
{
    using System;
    using System.Linq;

    public class Program : IProgram
    {
        private readonly ProgramSettings _settings;
        private readonly IOutput _output;

        public Program(ProgramSettings settings, IOutput output)
        {
            _settings = settings;
            _output = output;
        }
        static void Main()
        {
            var output = new ConsoleOutput();
            var programSettings = new ProgramSettings
            {
                //PlaceTestOrders = true
            };
            var program = new Program(programSettings, output);

            output.WriteLine("Starting Binance magic...");
            var actionValidator = new ActionValidator();
            var client = new Client(programSettings, program, actionValidator, output);
            client.Start();

            var allLoopSettings = new[]
            {
                new LoopSettings
                {
                    IsBackTest = true,
                    AllowedCoins = new []{ "BTC", "BNB" },
                    //InitialTransferFactor = 10.0m,
                    //MaxQuantityToTrade = 0.9m,
                    MinimalIncrease = 0.05m,
                },

                // new LoopSettings
                // {
                //     AllowedCoins = new []{ "BTC", "ZEN" },
                //     ReferenceCoin = "USDT",
                //     //InitialTransferFactor = 10.0m,
                //     //MaxQuantityToTrade = 0.9m,
                //     MinimalIncrease = 0.05m,
                // },

                // new LoopSettings
                // {
                //     AllowedCoins = new []{"ETH", "LTC"},
                //     InitialPurchaseMinimalFactor = 10.0m,
                //     MinimalIncrease = 0.05m,
                // },
            };

            var loops = allLoopSettings
                .Select(loopSettings =>
                {
                    var loop = CreateLoop(loopSettings, program, client, output);
                    loop.Start();
                    return loop;
                })
                .ToArray();

            Console.ReadLine();
            output.WriteLine("Stopping Binance magic...");
            foreach (var loop in loops)
            {
                loop.Stop();
            }
            output.WriteLine("Stopping Binance magic: Done");
        }
        
        public void HandleFail(string message)
        {
            if (_settings.IsTest)
            {
                throw new InvalidOperationException(message);
            }

            _output.WriteLineNegative(message);
            Environment.Exit(-1);
        }

        private static Loop CreateLoop(LoopSettings loopSettings, IProgram program, IClient realClient, IOutput output)
        {
            var client = loopSettings.IsBackTest
                ? new BackTestClient(loopSettings.AllowedCoins, loopSettings.ReferenceCoin, output)
                : realClient;
            var loop = new Loop(loopSettings, program, client, output);
            loop.Start();
            return loop;
        }
    }
}