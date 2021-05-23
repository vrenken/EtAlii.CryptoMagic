namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Reflection;
    using Serilog;

    public static class Logging
    {
       public static void Configure(LoggerConfiguration loggerConfiguration)
        {
            var executingAssemblyName = Assembly.GetCallingAssembly().GetName();

            loggerConfiguration.MinimumLevel.Verbose()
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
                .Enrich.WithProperty("UniqueProcessId", Guid.NewGuid()) // An int process ID is not enough
                .WriteTo.Async(writeTo =>
                {
                    writeTo.Seq("http://localhost:5341");
                    //writeTo.Seq("http://vrenken.duckdns.org:5341");
                    //writeTo.Seq("http://192.168.1.130:5341");
                });
        }
    }
}