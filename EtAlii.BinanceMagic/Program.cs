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

        public Loop CreateLoop(LoopSettings loopSettings, IProgram program, IOutput output)
        {
            var client = loopSettings.Client;
            if (client is not Client)
            {
                client.Start();
            }

            ISequence sequence = loopSettings.Algorithm switch
            {
                Circular.AlgorithmSettings algorithmSettings => new Circular.Sequence(algorithmSettings, program, client, output, loopSettings.Time),
                Surfing.AlgorithmSettings algorithmSettings => new Surfing.Sequence(algorithmSettings, client, output),
                _ => throw new InvalidOperationException("Unsupported algorithm")
            };

            var loop = new Loop(sequence);
            loop.Start();
            return loop;
        }
    }
}