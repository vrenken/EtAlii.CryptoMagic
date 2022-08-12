namespace EtAlii.CryptoMagic
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using Serilog;

    public static class Program
    {
        public static void Main(string[] args)
        {
            
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((_, builder) =>
                {
                    builder.ExpandEnvironmentVariablesInJson();
                    var configuration = builder.Build();
                    Logging.ConfigureGlobalLogging(configuration);
                })
                .UseSerilog((context, loggerConfiguration) => Logging.ConfigureWebHostLogging(context.Configuration, loggerConfiguration), true)
                .ConfigureWebHostDefaults(webHostBuilder =>
                {
                    webHostBuilder.UseStartup<Startup>();
                });
    }
}