// namespace EtAlii.BinanceMagic.Surfing
// {
//     using System;
//     using System.Threading;
//
//     public class Sequence : ISequence
//     {
//         private readonly AlgorithmSettings _settings;
//         public IStatusProvider Status => _status;
//         private StatusProvider _status;
//
//         private StateMachine _stateMachine;
//         private readonly TradeDetails _details;
//         private readonly Data _data;
//
//         public Sequence(AlgorithmSettings settings, IClient client, IOutput output)
//         {
//             _settings = settings;
//             _details = new TradeDetails();
//             _status = new StatusProvider(output, _details);
//             _data = new Data(client, settings, output);
//         }
//         
//         public void Run(CancellationToken cancellationToken)
//         {
//             _stateMachine = new StateMachine(_settings, _details, _data, _status, cancellationToken);
//             _stateMachine.Start();
//         }
//
//         public void Initialize()
//         {
//             //throw new NotImplementedException();
//         }
//     }
// }