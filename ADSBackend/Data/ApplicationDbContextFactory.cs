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
    
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
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
