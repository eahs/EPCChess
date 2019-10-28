using ADSBackend.Data;
using ADSBackend.Models;
using ADSBackend.Models.HomeViewModels;
using ADSBackend.Models.Identity;
using ADSBackend.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentSeason = await SeasonSelector.GetCurrentSeasonId(_context, HttpContext);
            int schoolId = await GetSchoolIdAsync();

            var viewModel = new HomeViewModel
            {
                User = await _userManager.GetUserAsync(User)
            };

            viewModel.Upcoming = await _context.Match.Where(m => m.MatchDate >= DateTime.Now && (m.HomeSchoolId == schoolId || m.AwaySchoolId == schoolId))
                                                     .OrderBy(m => m.MatchDate)
                                                     .Take(4)
                                                     .ToListAsync();

            //viewModel.HomeSchool = await _context.School.Where(s => s.SchoolId == schoolId).FirstOrDefaultAsync();

            viewModel.Divisions = await _context.Division.OrderBy(d => d.Name).ToListAsync();
            viewModel.TopSchoolPlayers = await _context.Player.Where(p => p.PlayerSchoolId == schoolId)
                                                              .OrderByDescending(p => p.Rating)
                                                              .ThenBy(p => p.LastName)
                                                              .ThenBy(p => p.FirstName)
                                                              .ToListAsync();

            Dictionary<int, School> scores = new Dictionary<int, School>();  // Maps schoolId to School

            var schools = await _context.School.Where(s => s.SeasonId == currentSeason)
                                               .OrderBy(s => s.Name)
                                               .ToListAsync();

            foreach (var school in schools)
            {
                scores.Add(school.SchoolId, school);
            }

            var matches = await _context.Match.Include(m => m.HomeSchool)
                                              .Include(m => m.AwaySchool)
                                              .Where(m => m.HomeSchool.SeasonId == currentSeason && m.AwaySchool.SeasonId == currentSeason && m.Completed)
                                              .ToListAsync();

            foreach (var match in matches)
            {
                scores[match.HomeSchoolId].Wins += match.HomePoints > match.AwayPoints ? 1 : 0;
                scores[match.HomeSchoolId].Losses += match.HomePoints < match.AwayPoints ? 1 : 0;
                scores[match.HomeSchoolId].Ties += match.HomePoints == match.AwayPoints ? 1 : 0;

                scores[match.AwaySchoolId].Wins += match.AwayPoints > match.HomePoints ? 1 : 0;
                scores[match.AwaySchoolId].Losses += match.AwayPoints < match.HomePoints ? 1 : 0;
                scores[match.AwaySchoolId].Ties += match.AwayPoints == match.HomePoints ? 1 : 0;

            }

            viewModel.HomeSchool = scores[schoolId];

            foreach (Division d in viewModel.Divisions)
            {
                d.Schools = schools.Where(s => s.DivisionId == d.DivisionId)
                                   .OrderByDescending(s => s.Points)
                                   .ThenBy(s => s.Name)
                                   .ToList();
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
