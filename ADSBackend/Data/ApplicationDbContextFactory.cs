
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ADSBackend.Data
{
    
    /// <summary>
    /// Factory for creating instances of <see cref="ApplicationDbContext"/> during design time.
    /// </summary>
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        /// <summary>
        /// Builds the application configuration.
        /// </summary>
        /// <returns>The application configuration root.</returns>
        public static IConfigurationRoot BuildConfiguration()
        {
            // Get environment
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            // Build config
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            return config;
        }

        /// <summary>
        /// Creates a new instance of the application database context.
        /// </summary>
        /// <param name="args">Arguments provided by the design-time service.</param>
        /// <returns>A new instance of <see cref="ApplicationDbContext"/>.</returns>
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var config = BuildConfiguration();

            // Get connection string
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connString = config.GetConnectionString("AppDatabaseContext");

            optionsBuilder.UseMySql(connString,
                                    new MySqlServerVersion(new Version(10, 3, 25))
                );

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
    
}