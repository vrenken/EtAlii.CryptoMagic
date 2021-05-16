namespace EtAlii.BinanceMagic.Service.Configuration
{
    using EtAlii.BinanceMagic.Service.Shared;

    public class Setting : Entity
    {
        public string Key { get; init; }
        public string Value { get; set; }
    }
}