﻿using ADSBackend.Data;
using ADSBackend.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADSBackend.Models;
using Microsoft.Extensions.DependencyInjection;
using ADSBackend.Services;
using Microsoft.EntityFrameworkCore;

namespace ADSBackend.Configuration
{
    public class ApplicationUserSeed : ISeeder
    {
        public void CreateAdminUser(UserManager<ApplicationUser> _userManager)
        {
            if (_userManager.FindByNameAsync("mike@logic-gate.com").Result != null)
            {
                return;
            }

            var adminUser = new ApplicationUser
            {
                UserName = "mike@logic-gate.com",
                FirstName = "Admin",
                Schools = new List<UserSchool>()
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

            adminUser.Schools.Add(new UserSchool
            {
                UserId = adminUser.Id,
                SchoolId = 1
            });
            _userManager.UpdateAsync(adminUser).Wait();

            _userManager.AddToRoleAsync(adminUser, "Admin").Wait();
        }

        // Temporarily used to migrate all users who have old school mappings to new style
        // Can be removed once the SchoolId field from ApplicationUser is removed
        public void MigrateUsers(ApplicationDbContext dbContext)
        {
            var users = dbContext.Users
                .Include(u => u.Schools)
                .ThenInclude(u => u.School)
                .Where(u => u.SchoolId != 1)
                .ToList();

            bool updated = false;

            foreach (var user in users)
            {
                var exists = user.Schools.FirstOrDefault(x => x.SchoolId == user.SchoolId);

                if (exists is null)
                {
                    updated = true;
                    user.Schools.Add(new UserSchool
                    {
                        UserId = user.Id,
                        SchoolId = user.SchoolId
                    });
                    user.SchoolId = 1;
                }
            }

            if (updated)
            {
                dbContext.SaveChanges();
            }
        }

        public Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            CreateAdminUser(userManager);
            MigrateUsers(dbContext);

            return Task.CompletedTask;
        }
    }
}
