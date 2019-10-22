using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADSBackend.Data;
using ADSBackend.Models.Identity;
using ADSBackend.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace ADSBackend.Controllers
{
    public class SchoolMatchesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SchoolMatchesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentSeason = await SeasonSelector.GetCurrentSeasonId(_context, HttpContext);
            var schoolId = await GetSchoolIdAsync();

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

        public async Task<IActionResult> Manage(int? id)
        {
            var currentSeason = await SeasonSelector.GetCurrentSeasonId(_context, HttpContext);
            var schoolId = await GetSchoolIdAsync();

            if (schoolId == -1)
                return NotFound();

            if (id == null)
            {
                return NotFound();
            }

            var match = await _context.Match.Include(m => m.HomeSchool).ThenInclude(m => m.Season)
                                                          .Include(m => m.AwaySchool).ThenInclude(m => m.Season)
                                                          .Include(m => m.Games).ThenInclude(g => g.HomePlayer)
                                                          .Include(m => m.Games).ThenInclude(g => g.AwayPlayer)
                                                          .Where(m => m.MatchId == id && m.HomeSchool.SeasonId == currentSeason && (m.HomeSchoolId == schoolId || m.AwaySchoolId == schoolId))
                                                          .FirstOrDefaultAsync();

            if (match == null)
            {
                return NotFound();
            }

            return View(match);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<String> UpdateRoster(IFormCollection forms)
        {
            // forms {p[]=1&p[]=2&p[]=7&p[]=3&p[]=4&p[]=5}
            await Task.Delay(1);

            string _id = "", _roster = "";
            int id;
            List<int?> roster;
            
            if (forms.ContainsKey("id"))
            {
                _id = forms["id"];
                if (!Int32.TryParse(_id, out id))
                    id = -1;
            }
            
            if (forms.ContainsKey("roster"))
            {
                _roster = forms["roster"];

                
                roster = _roster.Split(',')
                                .Select(i => int.TryParse(i, out int num) ? (int?)num : null)
                                .Where(i => i != null).ToList();

            }
            

            return "OK";
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