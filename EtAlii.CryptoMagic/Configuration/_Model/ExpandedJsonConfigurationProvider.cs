namespace EtAlii.CryptoMagic
{
    using System;
    using System.Linq;
    using Microsoft.Extensions.Configuration.Json;

    public class ExpandedJsonConfigurationProvider : JsonConfigurationProvider
    {
        public ExpandedJsonConfigurationProvider(ExpandedJsonConfigurationSource source)
            : base(source) { }

        public override void Load()
        {
            base.Load();
            Data = Data.ToDictionary(
                x => x.Key,
                x => Environment.ExpandEnvironmentVariables(x.Value),
                StringComparer.OrdinalIgnoreCase);
        }
    }
}