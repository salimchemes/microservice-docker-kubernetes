using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace VillaSport.MicroService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SetupSerilog();

            try
            {
                Log.Information("Starting web host");
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseStartup<Startup>();

        private static void SetupSerilog()
        {
            #if DEBUG
            Log.Logger = new LoggerConfiguration()
              .MinimumLevel.Debug()
              .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
              .Enrich.FromLogContext()
              .WriteTo.Console(theme: AnsiConsoleTheme.Code)
              .CreateLogger();
            #else

                // logging to application Insights. Needs to be tested once we have the AI id
                var configuration = new ConfigurationBuilder()
                 .AddJsonFile("appsettings.json")
                 .Build();

                new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .CreateLogger();
            #endif

        }
    }
}
