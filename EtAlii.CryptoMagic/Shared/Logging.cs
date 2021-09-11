namespace EtAlii.CryptoMagic
{
    using System;
    using System.Reflection;
    using Microsoft.Extensions.Configuration;
    using Serilog;

    public static class Logging
    {
       public static void Configure(IConfiguration configuration, LoggerConfiguration loggerConfiguration)
        {
            var executingAssembly = Assembly.GetCallingAssembly();
            var executingAssemblyName = executingAssembly.GetName();

            loggerConfiguration.ReadFrom
                .Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithThreadName()
                .Enrich.WithThreadId()
                .Enrich.WithProcessName()
                .Enrich.WithProcessId()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName()
                // These ones do not give elegant results during unit tests.
                // .Enrich.WithAssemblyName()
                // .Enrich.WithAssemblyVersion()
                // Let's do it ourselves.
                .Enrich.WithProperty("RootAssemblyName", executingAssemblyName.Name)
                .Enrich.WithProperty("RootAssemblyVersion", executingAssemblyName.Version)
                .Enrich.WithMemoryUsage()
                .Enrich.WithProperty("UniqueProcessId", Guid.NewGuid()); // An int process ID is not enough
        }
    }
}