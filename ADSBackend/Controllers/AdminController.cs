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
using Newtonsoft.Json;

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
        private readonly IEmailSender _mailSender;
        private readonly IRazorViewRenderer _razorViewRenderer;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, DataService dataService, IEmailSender mailSender, IRazorViewRenderer razorViewRenderer)
        {
            _context = context;
            _userManager = userManager;
            _dataService = dataService;
            _signInManager = signInManager;
            _mailSender = mailSender;
            _razorViewRenderer = razorViewRenderer;
        }

        public async Task<IActionResult> Index(int err)
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();
            int schoolId = await _dataService.GetSchoolIdAsync(User, currentSeason);

            var viewModel = new HomeViewModel
            {
                IsPlayerThisSeason = schoolId != -1,
                User = await _dataService.GetUserAsync(User),
                JoinCodeError = err > 0
            };

            bool isGuest = await _userManager.IsInRoleAsync(viewModel.User, "Guest");

            if (schoolId != -1)
            {
                if (!isGuest)
                    await _dataService.SyncExternalPlayer(viewModel.User.Id);


                viewModel.Upcoming = await _dataService.GetUpcomingMatchesAsync(currentSeason, schoolId, 4);

                viewModel.TopSchoolPlayers = await _context.Player.Include(p => p.PlayerSchool)
                    .Include(p => p.User)
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

            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(JoinViewModel model)
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();

            if (ModelState.IsValid && !String.IsNullOrEmpty(model.JoinCode))
            {
                model.JoinCode = model.JoinCode.ToUpper();

                var school = await _context.School.FirstOrDefaultAsync(s =>
                    s.SeasonId == currentSeason && s.JoinCode.Equals(model.JoinCode));

                if (school is not null)
                {
                    var appUser = await _dataService.GetUserAsync(User);
                    await _dataService.AddUserToSchoolAsync(appUser, school.SchoolId);

                    var identity = (User.Identity as ClaimsIdentity);
                    var guestClaim = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role && c.Value == "Guest");
                    if (guestClaim != null)
                        identity.RemoveClaim(guestClaim);
                    identity.AddClaim(new Claim(ClaimTypes.Role, "Player"));

                    if (User.IsInRole("Guest"))
                    {
                        // reset user roles
                        var roles = await _userManager.GetRolesAsync(appUser);
                        await _userManager.RemoveFromRolesAsync(appUser, roles);

                        // assign new role
                        await _userManager.AddToRoleAsync(appUser, "Player");

                        await _signInManager.SignInAsync(appUser, true);
                    }

                    await _dataService.SyncExternalPlayer(appUser.Id);

                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Index), new { err = 1 } );
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> JoinCodes()
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();
            var schools = await _context.School.Where(s => s.SeasonId == currentSeason).OrderBy(s => s.Name).ToListAsync();

            return View(schools);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SendCodes(int id)
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();
            var school = await _context.School.FirstOrDefaultAsync(s => s.SchoolId == id && s.SeasonId == currentSeason);

            if (school != null && !String.IsNullOrEmpty(school.AdvisorEmail))
            {
                var message = await _razorViewRenderer.RenderViewAsync("~/Views/Shared/EmailTemplates/JoinCodeMessage.cshtml", school);

                await _mailSender.SendEmailAsync(school.AdvisorEmail, "Welcome to EPC Chess!", message);

                return RedirectToAction("JoinCodes", new { result = "success" });
            }

            return RedirectToAction("JoinCodes", new { result = "error"});
        }

        [Authorize(Roles = "Admin")]
        public async Task<string> Dump ()
        {
            DumpViewModel dmp = new DumpViewModel
            {
                Matches = await _context.Match.Select(x => new 
                                                            { 
                                                                x.MatchId,
                                                                x.MatchDate,
                                                                x.HomePoints,
                                                                x.AwayPoints,
                                                                x.AwaySchoolId,
                                                                x.HomeSchoolId,
                                                                x.MatchStartTime,
                                                                x.AwayRosterLocked,
                                                                x.HomeRosterLocked,
                                                                x.ClockIncrement,
                                                                x.ClockTimeLimit,
                                                                x.IsVirtual,
                                                                MatchStarted = false,
                                                                Completed = false
                                                            })
                                               .OrderBy(m => m.MatchId)
                                               .Cast<object>()
                                               .ToListAsync(),

                Schools = await _context.School.Select(x => new
                                                            {
                                                                x.SchoolId,
                                                                x.DivisionId,
                                                                x.Name,
                                                                x.ShortName,
                                                                x.Abbreviation,
                                                                x.AdvisorName,
                                                                x.AdvisorEmail,
                                                                x.AdvisorPhoneNumber,
                                                                x.SeasonId,
                                                                x.JoinCode
                                                            })
                                                .OrderBy(m => m.SchoolId)
                                                .Cast<object>()
                                                .ToListAsync(),

                Seasons = await _context.Season.Select(x => new
                                                            {
                                                                x.SeasonId,
                                                                x.Name,
                                                                x.StartDate,
                                                                x.EndDate
                                                            })
                                                .OrderBy(m => m.SeasonId)
                                                .Cast<object>()
                                                .ToListAsync(),

                Divisions = await _context.Division.Select(x => new
                                                                {
                                                                    x.DivisionId,
                                                                    x.Name,
                                                                    x.SeasonId
                                                                })
                                                    .OrderBy(m => m.DivisionId)
                                                    .Cast<object>()
                                                    .ToListAsync(),
            };

            string json = JsonConvert.SerializeObject(dmp, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return json;
        }

    }

}
