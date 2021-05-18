// namespace EtAlii.BinanceMagic
// {
//     using System;
//     using System.Collections.Generic;
//     using System.Linq;
//     using Serilog;
//
//     public static class Startup
//     {
//         static void Main()
//         {
//             var loggerConfiguration = new LoggerConfiguration();
//             Logging.Configure(loggerConfiguration);
//             Log.Logger = loggerConfiguration.CreateLogger();
//
//             var output = new ConsoleOutput();
//             var programSettings = new ProgramSettings();
//             var program = new Program();
//
//             output.WriteLine("Starting Binance magic...");
//             var actionValidator = new ActionValidator();
//             var client = new Client(actionValidator)
//             {
//                 PlaceTestOrders = true,
//             };
//             
//             client.Start(programSettings.ApiKey, programSettings.SecretKey);
//
//             var allLoopSettings = new List<LoopSettings>();
//
//             // // Back-test 1.
//             var allowedCoins = new[] {"BTC", "BNB"};
//             var referenceCoin = "USDT";
//             var backTestClient = new BackTestClient(allowedCoins, referenceCoin, output, "");
//             allLoopSettings.Add(new LoopSettings
//             {
//                 Identifier = "BTC-BNB",
//                 Client = backTestClient,
//                 Time = new BackTestTimeManager
//                 {
//                     Client = backTestClient,
//                 },
//                 Algorithm = new Circular.AlgorithmSettings
//                 {
//                     AllowedCoins = allowedCoins,
//                     ReferenceCoin = referenceCoin,
//                     TargetIncrease = 1.03m,
//                     QuantityFactor = 10m,
//                     InitialTarget = 0.5m,
//                 },
//             });
//             
//             // // Back-test 1.
//             allowedCoins = new[] {"BTC", "XMR"};
//             referenceCoin = "USDT";
//             backTestClient = new BackTestClient(allowedCoins, referenceCoin, output, "");
//             allLoopSettings.Add(new LoopSettings
//             {
//                 Identifier = "BTC-XMR",
//                 Client = backTestClient,
//                 Time = new BackTestTimeManager
//                 {
//                     Client = backTestClient,
//                 },
//                 Algorithm = new Circular.AlgorithmSettings
//                 {
//                     AllowedCoins = allowedCoins,
//                     ReferenceCoin = referenceCoin,
//                     TargetIncrease = 1.03m,
//                     QuantityFactor = 10m,
//                     InitialTarget = 0.5m,
//                 }
//             });
//
//             // Back-test 2.
//             // var allowedCoins = new[] {"LTC", "DASH"};
//             // var referenceCoin = "USDT";
//             // var backTestClient = new BackTestClient(allowedCoins, referenceCoin, output);
//             // var time = new BackTestTimeManager(backTestClient, program);
//             // allLoopSettings.Add(new LoopSettings
//             // {
//             //     Client = backTestClient,
//             //     Time = time,
//             //     Algorithm = new CircularAlgorithmSettings
//             //     {
//             //         AllowedCoins = allowedCoins,
//             //         ReferenceCoin = referenceCoin,
//             //         TargetIncrease = 1.05m,
//             //         InitialTarget = 0.5m,
//             //         WriteTrends = false,
//             //     }
//             // });
//
//             // Back-test 3.
//             // var allowedCoins = new[] {"BTC", "ETH"};
//             // var referenceCoin = "USDT";
//             // var backTestClient = new BackTestClient(allowedCoins, referenceCoin, output);
//             // var time = new BackTestTimeManager(backTestClient, program);
//             // allLoopSettings.Add(new LoopSettings
//             // {
//             //     Client = backTestClient,
//             //     Time = time,
//             //     Algorithm = new CircularAlgorithmSettings
//             //     {
//             //         AllowedCoins = allowedCoins,
//             //         ReferenceCoin = referenceCoin,
//             //         TargetIncrease = 1.05m,
//             //         InitialTarget = 0.5m,
//             //         WriteTrends = false,
//             //     }
//             // });
//
//             /*
//             // Live test 1
//             allLoopSettings.Add(new LoopSettings
//             {
//                 Client = client,
//                 Time = new RealtimeTimeManager(),
//                 Algorithm = new Circular.AlgorithmSettings
//                 {
//                     AllowedCoins = new[] {"BTC", "BNB"},
//                     ReferenceCoin = "USDT",
//                     TargetIncrease = 1.03m,
//                     InitialTarget = 0.5m,
//                     QuantityFactor = 10m,
//                     SampleInterval = TimeSpan.FromMinutes(1),
//                     WriteTrends = false,
//                 }
//             });
//
//             // Live test 2
//             allLoopSettings.Add(new LoopSettings
//             {
//                 Client = client,
//                 Time = new RealtimeTimeManager(),
//                 Algorithm = new Circular.AlgorithmSettings
//                 {
//                     AllowedCoins = new[] {"BTC", "XMR"},
//                     ReferenceCoin = "USDT",
//                     TargetIncrease = 1.03m,
//                     InitialTarget = 0.5m,
//                     QuantityFactor = 10m,
//                     SampleInterval = TimeSpan.FromMinutes(1),
//                     WriteTrends = false,
//                 }
//             });
//             */
//             
//             // Live test 3
//             allLoopSettings.Add(new LoopSettings
//             {
//                 Identifier = "BTC-BNB-LTC-XMR-ADA-RUNE",
//                 Client = client,
//                 Time = new RealtimeTimeManager(),
//                 Algorithm = new Surfing.AlgorithmSettings
//                 {
//                     AllowedCoins = new[] { "BTC", "BNB", "LTC", "XMR", "ADA", "RUNE" }, // "ETH", 
//                     PayoutCoin = "USDT",
//                     ActionInterval = TimeSpan.FromMinutes(1), // TimeSpan.FromSeconds(20),// 
//                     InitialPurchase = 100m, // in USDT
//                     TransferFactor = 0.995m,
//                     RsiOverSold = 0.60m,
//                     RsiOverBought = 0.70m,
//                     RsiPeriod = 6,
//                 }
//             });
//
//             var loops = allLoopSettings
//                 .Select(ls => program.CreateLoop(ls, programSettings, output))
//                 .ToArray();
//
//             void OnStatusChanged(StatusInfo statusInfo)
//             {
//                 //if (statusInfo.HasFlag(StatusInfo.Important))
//                 {
//                     Console.Clear();
//
//                     foreach (var loop in loops)
//                     {
//                         loop.Status.Write();
//                     }
//                 }
//             }
//             foreach (var loop in loops)
//             {
//                 loop.Status.Changed += OnStatusChanged;
//                 loop.Start();
//
//             }
//             Console.ReadLine();
//             output.WriteLine("Stopping Binance magic...");
//             foreach (var loop in loops)
//             {
//                 loop.Stop();
//             }
//             
//             client.Stop();
//
//             output.WriteLine("Stopping Binance magic: Done");
//         }
//     }
// }