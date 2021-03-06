﻿using ADSBackend.Data;
using ADSBackend.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ADSBackend.Services;

namespace ADSBackend.Configuration
{
    public class ApplicationUserSeed : ISeeder
    {
        public void CreateAdminUser(UserManager<ApplicationUser> _userManager)
        {
            if (_userManager.FindByNameAsync("admin").Result != null)
            {
                return;
            }

            var adminUser = new ApplicationUser
            {
                UserName = "admin",
                FirstName = "Admin",
                SchoolId = 1
            };

            IdentityResult result;
            try
            {
                result = _userManager.CreateAsync(adminUser, "Password123!").Result;
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while creating the admin user: " + e.InnerException);
            }

            if (!result.Succeeded)
            {
                throw new Exception("The following error(s) occurred while creating the admin user: " + string.Join(" ", result.Errors.Select(e => e.Description)));
            }

            _userManager.AddToRoleAsync(adminUser, "Admin").Wait();
        }

        public Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            CreateAdminUser(userManager);

            return Task.CompletedTask;
        }
    }
}
