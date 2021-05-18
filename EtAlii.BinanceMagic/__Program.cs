// namespace EtAlii.BinanceMagic
// {
//     using System;
//
//     public class Program
//     {
//         public Loop CreateLoop(LoopSettings loopSettings, ProgramSettings programSettings, IOutput output)
//         {
//             var client = loopSettings.Client;
//             if (client is not Client)
//             {
//                 client.Start(programSettings.ApiKey, programSettings.SecretKey);
//             }
//
//             ISequence sequence;
//             switch(loopSettings.Algorithm) 
//             {
//                 case Circular.AlgorithmSettings algorithmSettings:
//                     var tradeDetailsPersistence = new Persistence<Circular.TradeDetails>(programSettings.StorageFolder, loopSettings.Identifier);
//                     sequence = new Circular.Sequence(algorithmSettings, client, output, loopSettings.Time, tradeDetailsPersistence);
//                     break;
//                 case Surfing.AlgorithmSettings algorithmSettings:
//                     var transactionPersistence = new Persistence<Transaction>(programSettings.StorageFolder, loopSettings.Identifier);
//                     sequence = new Surfing.Sequence(algorithmSettings, client, output, loopSettings.Time, transactionPersistence);
//                     break;
//                 default:
//                     throw new InvalidOperationException("Unsupported algorithm");
//             }
//
//             return new Loop(sequence);
//         }
//     }
// }