namespace EtAlii.BinanceMagic
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public class AutomationLoop
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private Task _task;
        private readonly MagicAlgorithm _algorithm;
        private readonly Client _client;

        public AutomationLoop()
        {
            _client = new();
            _algorithm = new(_client);
        }
        
        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _task.Wait();
        }

        public void Start()
        {
            _task = Task.Run(Run);
        }
        private void Run()
        {
            var cancellationToken = _cancellationTokenSource.Token;

            _client.Start();

            _algorithm.Load();

            while (!cancellationToken.IsCancellationRequested)
            {
                var target = _algorithm.BuildTarget(cancellationToken);
                ConsoleOutput.Write($"Found next target: {target.SourceCoin} -> {target.TargetCoin} using minimal increase: {MagicAlgorithm.MinimalIncrease:P}");

                var targetSucceeded = false;

                while (!targetSucceeded)
                {
                    var situation = _algorithm.GetSituation(target, cancellationToken);
                    ConsoleOutput.Write($"Current situation:");
                    ConsoleOutput.Write($"{target.SourceCoin}");
                    ConsoleOutput.Write($"- Past    = {situation.SourceDelta.PastPrice * situation.SourceDelta.PastQuota} {MagicAlgorithm.ReferenceCoin}");
                    ConsoleOutput.Write($"- Present = {situation.SourceDelta.PresentPrice * situation.SourceDelta.PastQuota} {MagicAlgorithm.ReferenceCoin}");
                    ConsoleOutput.Write($"- Fee     = {situation.SourceSellFee:P}");
                    ConsoleOutput.Write($"{target.TargetCoin}");
                    ConsoleOutput.Write($"- Past    = {situation.TargetDelta.PastPrice} {MagicAlgorithm.ReferenceCoin}");
                    ConsoleOutput.Write($"- Present = {situation.TargetDelta.PresentPrice} {MagicAlgorithm.ReferenceCoin}");
                    ConsoleOutput.Write($"- Fee     = {situation.TargetBuyFee:P}");
                    ConsoleOutput.Write($"- Target  = {target.MinimalRequiredGain} {MagicAlgorithm.ReferenceCoin}");
                    
                        
                    if (_algorithm.TransactionIsWorthIt(target, situation))
                    {
                        ConsoleOutput.Write($"Feasible transaction found - Converting...");
                        if (_client.TryConvert(target, situation))
                        {
                            ConsoleOutput.WritePositive($"Transaction done!");
                            WriteGamble(situation);
                            WriteProfit(situation);
                            targetSucceeded = true;
                        }
                    }
                    else
                    {
                        var interval = MagicAlgorithm.SampleInterval;
                        ConsoleOutput.WriteNegative($"No feasible transaction - Waiting until: {DateTime.Now + interval}");
                        Task.Delay(interval, cancellationToken).Wait(cancellationToken);
                    }
                }
            }

            _client.Stop();
        }

        private void WriteGamble(Situation situation)
        {
            using var file = new FileStream(MagicAlgorithm.GambleFile, FileMode.Append, FileAccess.Write, FileShare.Read);
            using var sw = new StreamWriter(file);

            sw.Write("Hello");
        }

        private void WriteProfit(Situation situation)
        {
            using var file = new FileStream(MagicAlgorithm.ProfitFile, FileMode.Append, FileAccess.Write, FileShare.Read);
            using var sw = new StreamWriter(file);
            sw.Write("Hello");
        }
    }
}