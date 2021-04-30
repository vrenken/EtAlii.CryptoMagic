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
                // new LoopSettings
                // {
                //     IsBackTest = true,
                //     Algorithm = new CircularAlgorithmSettings
                //     {
                //         AllowedCoins = new []{ "BTC", "BNB" },
                //         //InitialTransferFactor = 10.0m,
                //         //MaxQuantityToTrade = 0.9m,
                //         MinimalIncrease = 0.05m,
                //     }
                // },

                new LoopSettings
                {
                    Algorithm = new CircularAlgorithmSettings
                    {
                        AllowedCoins = new []{ "BTC", "ZEN" },
                        ReferenceCoin = "USDT",
                        //InitialTransferFactor = 10.0m,
                        //MaxQuantityToTrade = 0.9m,
                        MinimalIncrease = 0.05m,
                    }
                },

                // new LoopSettings
                // {
                //     Algorithm = new CircularAlgorithmSettings
                //     {
                //         AllowedCoins = new []{"ETH", "LTC"},
                //         InitialPurchaseMinimalFactor = 10.0m,
                //         MinimalIncrease = 0.05m,
                //     }
                // },
            };

            var loops = allLoopSettings
                .Select(loopSettings => CreateLoop(loopSettings, program, client, output))
                .ToArray();

            void OnStatusChanged()
            {
                Console.Clear();

                foreach (var loop in loops)
                {
                    loop.Status.Write();
                }
            }
            foreach (var loop in loops)
            {
                loop.Status.Changed += OnStatusChanged;
                loop.Start();

            }
            Console.ReadLine();
            output.WriteLine("Stopping Binance magic...");
            foreach (var loop in loops)
            {
                loop.Stop();
            }
            
            client.Stop();

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

        private static Loop CreateLoop(LoopSettings loopSettings, IProgram program, IClient client, IOutput output)
        {
            switch (loopSettings.Algorithm)
            {
                case CircularAlgorithmSettings algorithmSettings:
                    if (loopSettings.IsBackTest)
                    {
                        client = new BackTestClient(algorithmSettings.AllowedCoins, algorithmSettings.ReferenceCoin, output);
                        client.Start();
                    }

                    var sequence = new CircularSequence(algorithmSettings, program, client, output);
                    var loop = new Loop(sequence);
                    loop.Start();
                    return loop;
                default:
                    throw new InvalidOperationException("Unsupported algorithm");
            }
            
        }
    }
}