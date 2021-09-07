namespace EtAlii.CryptoMagic
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Configuration.Json;

    public class ExpandedJsonConfigurationSource : JsonConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new ExpandedJsonConfigurationProvider(this);
        }
    }
}