namespace EtAlii.CryptoMagic
{
    using System.Linq;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Configuration.Json;

    public static class ConfigurationBuilderExtension
    {
        public static void ExpandEnvironmentVariablesInJson(this IConfigurationBuilder builder)
        {
            var jsonConfigurationSources = builder.Sources
                .OfType<JsonConfigurationSource>()
                .ToList();

            foreach (var jsonConfigurationSource in jsonConfigurationSources)
            {
                var indexOfJsonConfigurationSource = builder.Sources
                    .IndexOf(jsonConfigurationSource);

                builder.Sources.RemoveAt(indexOfJsonConfigurationSource);
                builder.Sources.Insert(
                    indexOfJsonConfigurationSource,
                    new ExpandedJsonConfigurationSource
                    {
                        FileProvider = jsonConfigurationSource.FileProvider,
                        Path = jsonConfigurationSource.Path,
                        Optional = jsonConfigurationSource.Optional,
                        ReloadOnChange = jsonConfigurationSource.ReloadOnChange
                    });
            }
        }
    }
}