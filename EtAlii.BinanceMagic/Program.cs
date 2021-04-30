namespace EtAlii.BinanceMagic
{
    using System;
    using System.Linq;

    public class Program : IProgram
    {
        private readonly ProgramSettings _settings;

        public Program(ProgramSettings settings)
        {
            _settings = settings;
        }
        static void Main()
        {
            var programSettings = new ProgramSettings
            {
                //PlaceTestOrders = true
            };
            var program = new Program(programSettings);

            ConsoleOutput.Write("Starting Binance magic...");
            var actionValidator = new ActionValidator();
            var client = new Client(programSettings, program, actionValidator);
            client.Start();

            var allLoopSettings = new[]
            {
                // new LoopSettings
                // {
                //     AllowedCoins = new []{ "BTC", "BNB" },
                //     //InitialTransferFactor = 10.0m,
                //     //MaxQuantityToTrade = 0.9m,
                //     MinimalIncrease = 0.05m,
                // },

                new LoopSettings
                {
                    AllowedCoins = new []{ "BTC", "ZEN" },
                    ReferenceCoin = "USDT",
                    //InitialTransferFactor = 10.0m,
                    //MaxQuantityToTrade = 0.9m,
                    MinimalIncrease = 0.05m,
                },

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
                    var loop = new Loop(loopSettings, program, client);
                    loop.Start();
                    return loop;
                })
                .ToArray();

            Console.ReadLine();
            ConsoleOutput.Write("Stopping Binance magic...");
            foreach (var loop in loops)
            {
                loop.Stop();
            }
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