namespace EtAlii.BinanceMagic
{
    public class LoopSettings
    {
        public string Identifier { get; init; }
        public IClient Client { get; init; }
        public ITimeManager Time { get; init; }
        public IAlgorithmSettings Algorithm { get; init; }
    }
}