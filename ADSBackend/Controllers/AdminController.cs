using ADSBackend.Data;
using ADSBackend.Models;
using ADSBackend.Models.HomeViewModels;
using ADSBackend.Models.Identity;
using ADSBackend.Services;
using ADSBackend.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace ADSBackend.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(RefreshTokenFilter))]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly DataService _dataService;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, DataService dataService)
        {
            _context = context;
            _userManager = userManager;
            _dataService = dataService;
        }

        public async Task<IActionResult> Index()
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();
            int schoolId = await GetSchoolIdAsync();

            var viewModel = new HomeViewModel
            {
                User = await _userManager.GetUserAsync(User)
            };

            await _dataService.SyncExternalPlayer(viewModel.User.Id);


            viewModel.Upcoming = await _dataService.GetUpcomingMatchesAsync(currentSeason, schoolId, 4);

            viewModel.TopSchoolPlayers = await _context.Player.Include(p => p.PlayerSchool)
                                                              .Where(p => p.PlayerSchoolId == schoolId && p.PlayerSchool.SeasonId == currentSeason)
                                                              .OrderByDescending(p => p.Rating)
                                                              .ThenBy(p => p.LastName)
                                                              .ThenBy(p => p.FirstName)
                                                              .ToListAsync();

            viewModel.Divisions = await _dataService.GetDivisionStandingsAsync(currentSeason);

            foreach (Division d in viewModel.Divisions)
            {
                var homeschool = d.Schools.FirstOrDefault(s => s.SchoolId == schoolId);

                if (homeschool != null)
                {
                    viewModel.HomeSchool = homeschool;
                    break;
                }
            }

            if (viewModel.HomeSchool == null)
            {
                viewModel.HomeSchool = new School
                {
                    Name = "Unassigned",
                    ShortName = "Unassigned",
                    Abbreviation = "Unassigned",
                    AdvisorName = "",
                    AdvisorEmail = "",
                    AdvisorPhoneNumber = ""
                };
            }

            return View(viewModel);
        }

        private async Task<int> GetSchoolIdAsync()
        {
            
            var user = await _userManager.GetUserAsync(User);
             
            if (user == null)
                return -1;

            return user.SchoolId;
        }
    }

}
