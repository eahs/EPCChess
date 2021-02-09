using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADSBackend.Data;
using ADSBackend.Models.Identity;
using ADSBackend.Models.PlayViewModels;
using ADSBackend.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ADSBackend.Controllers
{
    public class PlayController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DataService _dataService;

        public PlayController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, DataService dataService)
        {
            _context = context;
            _userManager = userManager;
            _dataService = dataService;
        }

        public async Task<IActionResult> Index()
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();
            var schoolId = await _dataService.GetSchoolIdAsync(User);

            if (schoolId == -1)
                return NotFound();

            var matches = await _context.Match.Include(m => m.HomeSchool).ThenInclude(m => m.Season)
                .Include(m => m.AwaySchool).ThenInclude(m => m.Season)
                .Where(m => m.HomeSchool.SeasonId == currentSeason && (m.HomeSchoolId == schoolId || m.AwaySchoolId == schoolId))
                .OrderBy(m => m.MatchDate)
                .ToListAsync();

            ViewBag.School = await _context.School.FirstOrDefaultAsync(m => m.SchoolId == schoolId);

            return View(matches);
        }

        public async Task<IActionResult> Match(int? id)
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();
            var schoolId = await _dataService.GetSchoolIdAsync(User);

            if (schoolId == -1)
                return NotFound();

            if (id == null)
            {
                return NotFound();
            }

            var match = await _dataService.GetMatchAsync(id, currentSeason, schoolId);

            if (match == null)
            {
                return NotFound();
            }

            MatchViewModel viewmodel = new MatchViewModel
            {
                Match = match
            };

            return View(viewmodel);
        }
    }
}
