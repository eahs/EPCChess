using ADSBackend.Data;
using ADSBackend.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADSBackend.Models;
using Microsoft.Extensions.DependencyInjection;
using ADSBackend.Services;

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

        public Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            CreateAdminUser(userManager);

            return Task.CompletedTask;
        }
    }
}
