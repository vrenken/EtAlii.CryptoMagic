namespace EtAlii.BinanceMagic
{
    using System;
    using System.Collections.Generic;
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
            var programSettings = new ProgramSettings();
            var program = new Program(programSettings, output);

            output.WriteLine("Starting Binance magic...");
            var actionValidator = new ActionValidator();
            var client = new Client(programSettings, program, actionValidator, output)
            {
                PlaceTestOrders = false,
            };
            
            client.Start();

            var allLoopSettings = new List<LoopSettings>();

            // // Back-test 1.
            // var allowedCoins = new[] {"BTC", "XMR"};
            // var referenceCoin = "USDT";
            // var backTestClient = new BackTestClient(allowedCoins, referenceCoin, output);
            // var time = new BackTestTimeManager(backTestClient, program);
            // allLoopSettings.Add(new LoopSettings
            // {
            //     Client = backTestClient,
            //     Time = time,
            //     Algorithm = new CircularAlgorithmSettings
            //     {
            //         AllowedCoins = allowedCoins,
            //         ReferenceCoin = referenceCoin,
            //         TargetIncrease = 1.03m,
            //         QuantityFactor = 10m,
            //         InitialTarget = 0.5m,
            //         WriteTrends = false,
            //     }
            // });

            // Back-test 2.
            // var allowedCoins = new[] {"LTC", "DASH"};
            // var referenceCoin = "USDT";
            // var backTestClient = new BackTestClient(allowedCoins, referenceCoin, output);
            // var time = new BackTestTimeManager(backTestClient, program);
            // allLoopSettings.Add(new LoopSettings
            // {
            //     Client = backTestClient,
            //     Time = time,
            //     Algorithm = new CircularAlgorithmSettings
            //     {
            //         AllowedCoins = allowedCoins,
            //         ReferenceCoin = referenceCoin,
            //         TargetIncrease = 1.05m,
            //         InitialTarget = 0.5m,
            //         WriteTrends = false,
            //     }
            // });

            // Back-test 3.
            // var allowedCoins = new[] {"BTC", "ETH"};
            // var referenceCoin = "USDT";
            // var backTestClient = new BackTestClient(allowedCoins, referenceCoin, output);
            // var time = new BackTestTimeManager(backTestClient, program);
            // allLoopSettings.Add(new LoopSettings
            // {
            //     Client = backTestClient,
            //     Time = time,
            //     Algorithm = new CircularAlgorithmSettings
            //     {
            //         AllowedCoins = allowedCoins,
            //         ReferenceCoin = referenceCoin,
            //         TargetIncrease = 1.05m,
            //         InitialTarget = 0.5m,
            //         WriteTrends = false,
            //     }
            // });

                
            // Live test 1
            allLoopSettings.Add(new LoopSettings
            {
                Client = client,
                Time = new RealtimeTimeManager(),
                Algorithm = new CircularAlgorithmSettings
                {
                    AllowedCoins = new[] {"BTC", "BNB"},
                    ReferenceCoin = "USDT",
                    TargetIncrease = 1.03m,
                    InitialTarget = 0.5m,
                    QuantityFactor = 10m,
                    SampleInterval = TimeSpan.FromMinutes(1),
                    WriteTrends = false,
                }
            });

            // Live test 2
            // allLoopSettings.Add(new LoopSettings
            // {
            //     Client = new BackTestClient(new []{ "ETH", "LTC" }, "BUSD", output, program),
            //     Time = new RealtimeTimeManager(),
            //     Algorithm = new CircularAlgorithmSettings
            //     {
            //         AllowedCoins = new []{"ETH", "LTC"},
            //     }
            // });

            var loops = allLoopSettings
                .Select(ls => CreateLoop(ls, program, output))
                .ToArray();

            void OnStatusChanged(StatusInfo statusInfo)
            {
                //if (statusInfo.HasFlag(StatusInfo.Important))
                {
                    Console.Clear();

                    foreach (var loop in loops)
                    {
                        loop.Status.Write();
                    }
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
        public void HandleFinish(string message)
        {
            _output.WriteLine(message);
            Console.ReadLine();
            Environment.Exit(-1);
        }

        private static Loop CreateLoop(LoopSettings loopSettings, IProgram program, IOutput output)
        {
            var client = loopSettings.Client;
            if (client is not Client)
            {
                client.Start();
            }

            switch (loopSettings.Algorithm)
            {
                case CircularAlgorithmSettings algorithmSettings:
                    var sequence = new CircularSequence(algorithmSettings, program, client, output, loopSettings.Time);
                    var loop = new Loop(sequence);
                    loop.Start();
                    return loop;
                default:
                    throw new InvalidOperationException("Unsupported algorithm");
            }
        }
    }
}