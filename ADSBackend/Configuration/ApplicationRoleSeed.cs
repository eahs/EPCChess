
using ADSBackend.Data;
using ADSBackend.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ADSBackend.Services;

namespace ADSBackend.Configuration
{
    /// <summary>
    /// Seeds the database with initial roles.
    /// </summary>
    public class ApplicationRoleSeed : ISeeder
    {
        /// <summary>
        /// Creates the initial roles if they do not exist.
        /// </summary>
        /// <param name="_roleManager">The role manager.</param>
        public void CreateRoles(RoleManager<ApplicationRole> _roleManager)
        {
            var roles = new List<string>
            {
                "Admin",
                "Advisor",
                "Player",
                "Guest"
            };

            foreach (var roleName in roles)
            {
                if (!_roleManager.RoleExistsAsync(roleName).Result)
                {
                    var role = new ApplicationRole { Name = roleName };

                    _roleManager.CreateAsync(role).Wait();
                }
            }
        }

        /// <summary>
        /// Asynchronously seeds the roles into the database.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>A task that represents the asynchronous seed operation.</returns>
        public Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            CreateRoles(roleManager);

            return Task.CompletedTask;
        }
    }
}