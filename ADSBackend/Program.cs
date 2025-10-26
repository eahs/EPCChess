
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
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Gets or sets the application configuration.
        /// </summary>
        public static IConfigurationRoot AppConfiguration { get; set; }

        /// <summary>
        /// The main entry point method.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
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

        /// <summary>
        /// Builds the web host for the application.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        /// <returns>An <see cref="IWebHost"/> instance.</returns>
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