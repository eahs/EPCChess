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
using ADSBackend.Models.AdminViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace ADSBackend.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(RefreshTokenFilter))]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly DataService _dataService;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, DataService dataService)
        {
            _context = context;
            _userManager = userManager;
            _dataService = dataService;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index(int err)
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();
            int schoolId = await GetSchoolIdAsync();

            var viewModel = new HomeViewModel
            {
                User = await _userManager.GetUserAsync(User),
                JoinCodeError = err > 0
            };

            bool isGuest = await _userManager.IsInRoleAsync(viewModel.User, "Guest");

            if (!isGuest)
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

        [HttpPost]
        public async Task<IActionResult> Index(JoinViewModel model)
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();

            if (ModelState.IsValid)
            {
                model.JoinCode = model.JoinCode.ToUpper();

                var school = await _context.School.FirstOrDefaultAsync(s =>
                    s.SeasonId == currentSeason && s.JoinCode.Equals(model.JoinCode));

                if (school is not null)
                {
                    var appUser = await _userManager.GetUserAsync(User);
                    appUser.SchoolId = school.SchoolId;

                    var identity = (User.Identity as ClaimsIdentity);
                    var guestClaim = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role && c.Value == "Guest");
                    if (guestClaim != null)
                        identity.RemoveClaim(guestClaim);
                    identity.AddClaim(new Claim(ClaimTypes.Role, "Player"));

                    // reset user roles
                    var roles = await _userManager.GetRolesAsync(appUser);
                    await _userManager.RemoveFromRolesAsync(appUser, roles);

                    // assign new role
                    await _userManager.AddToRoleAsync(appUser, "Player");

                    await _signInManager.SignInAsync(appUser, true);

                    await _dataService.SyncExternalPlayer(appUser.Id);

                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Index), new { err = 1 } );
            }

            return RedirectToAction(nameof(Index));
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
