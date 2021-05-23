// #nullable enable
// #pragma warning disable SL2001
//
// namespace EtAlii.BinanceMagic.Service
// {
//     using System;
//     using System.Threading;
//     using System.Threading.Tasks;
//
//     public partial class CircularSequence : CircularSequenceBase, ISequence
//     {
//         private readonly AlgorithmSettings _settings;
//         private readonly IClient _client;
//         private readonly Data _data;
//         private readonly ITimeManager _timeManager;
//         private readonly CircularTransaction _transaction;
//         
//         public IStatusProvider Status => _status;
//         private readonly StatusProvider _status;
//
//         private CancellationToken _cancellationToken;
//         private Situation? _situation;
//
//         public CircularSequence(AlgorithmSettings settings, IClient client, IOutput output, ITimeManager timeManager)
//         {
//             _settings = settings;
//             _client = client;
//             _timeManager = timeManager;
//         }
//
//         public void Run(CancellationToken cancellationToken)
//         {
//             Task.Delay(TimeSpan.FromSeconds(30), cancellationToken).Wait(cancellationToken);
//         }
//         public void Initialize(CancellationToken cancellationToken)
//         {
//             _cancellationToken = cancellationToken;
//             Start();
//         }
//     }
// }