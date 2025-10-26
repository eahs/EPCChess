
using ADSBackend.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Services
{
    /// <summary>
    /// Interface for a database seeder.
    /// </summary>
    public interface ISeeder
    {
        /// <summary>
        /// Asynchronously seeds the database.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>A task that represents the asynchronous seed operation.</returns>
        Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider);
    }
}