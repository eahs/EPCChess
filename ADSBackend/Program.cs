using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using ADSBackend.Data;

namespace ADSBackend
{
    public class Program
    {
        public static IConfigurationRoot AppConfiguration { get; set; }

        public static void Main(string[] args)
        {
            string logPath = "Logs" + Path.DirectorySeparatorChar;
            Directory.CreateDirectory(logPath);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Debug()
                .WriteTo.File(logPath, rollingInterval: RollingInterval.Day,
                                        flushToDiskInterval: TimeSpan.FromSeconds(1),
                                        shared: true)
                .CreateLogger();


            try
            {
                Log.Information("Starting web host");
                BuildWebHost(args).Run();
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

        public static IWebHost BuildWebHost(string[] args)
        {
            // Build an app configuration that includes environment-based appsettings
            AppConfiguration = ApplicationDbContextFactory.BuildConfiguration();

            return WebHost.CreateDefaultBuilder(args)
                          .UseConfiguration(AppConfiguration)
                          .UseStartup<Startup>()
                          .Build();
        }
    }
}
