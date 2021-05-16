namespace EtAlii.BinanceMagic.Service
{
    public class Setting : Entity
    {
        public string Key { get; init; }
        public string Value { get; set; }
    }

    public static class SettingKey
    {
        public const string ApiKey = "ApiKey";
        public const string SecretKey = "SecretKey";
    }
}