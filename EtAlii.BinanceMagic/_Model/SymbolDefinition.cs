namespace EtAlii.BinanceMagic
{
    public class SymbolDefinition 
    {
        public string Name { get; init;}
        public string Base { get; init; }

        public override string ToString() => $"{Name}/{Base}";
    }
}