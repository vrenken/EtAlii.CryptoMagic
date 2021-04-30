namespace EtAlii.BinanceMagic
{
    public class LoopSettings
    {
        public bool IsBackTest { get; init; }
        
        public IAlgorithmSettings Algorithm { get; init; }
    }
}