namespace EtAlii.BinanceMagic
{
    public class LoopSettings
    {
        public IClient Client { get; init; }
        public ITimeManager Time { get; init; }
        public IAlgorithmSettings Algorithm { get; init; }
        public Persistence<Transaction> Persistence { get; init; }
    }
}