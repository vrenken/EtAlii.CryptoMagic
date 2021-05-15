namespace EtAlii.BinanceMagic
{
    using System;

    public class Program : IProgram
    {
        private readonly ProgramSettings _settings;
        private readonly IOutput _output;

        public Program(ProgramSettings settings, IOutput output)
        {
            _settings = settings;
            _output = output;
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

        public Loop CreateLoop(LoopSettings loopSettings, ProgramSettings programSettings, IProgram program, IOutput output)
        {
            var client = loopSettings.Client;
            if (client is not Client)
            {
                client.Start();
            }

            ISequence sequence;
            switch(loopSettings.Algorithm) 
            {
                case Circular.AlgorithmSettings algorithmSettings:
                    var tradeDetailsPersistence = new Persistence<Circular.TradeDetails>(programSettings, loopSettings.Identifier);
                    sequence = new Circular.Sequence(algorithmSettings, program, client, output, loopSettings.Time, tradeDetailsPersistence);
                    break;
                case Surfing.AlgorithmSettings algorithmSettings:
                    var transactionPersistence = new Persistence<Transaction>(programSettings, loopSettings.Identifier);
                    sequence = new Surfing.Sequence(algorithmSettings, client, output, loopSettings.Time, transactionPersistence);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported algorithm");
            }

            return new Loop(sequence);
        }
    }
}