
using ADSBackend.Configuration;
using ADSBackend.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ADSBackend.Services
{
    /// <summary>
    /// Main seeder class that orchestrates other seeders.
    /// </summary>
    public class ApplicationSeeder : ISeeder
    {
        /// <summary>
        /// Asynchronously seeds the database by running all registered seeders.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>A task that represents the asynchronous seed operation.</returns>
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var seeders = new List<ISeeder>
            {
                new ApplicationDbSeed(),
                new ApplicationRoleSeed(),
                new ApplicationUserSeed()
            };

            foreach (var seeder in seeders)
            {
                await seeder.SeedAsync(dbContext, serviceProvider);
                await dbContext.SaveChangesAsync();
                Log.Information($"Seeder {seeder.GetType().Name} done.");
            }
        }

        private List<T> GetInstances<T>()
        {
            return (from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.GetInterfaces().Contains(typeof(T)) && t.GetConstructor(Type.EmptyTypes) != null && t.Name != this.GetType().Name
                    select (T)Activator.CreateInstance(t)).ToList();
        }
    }
}