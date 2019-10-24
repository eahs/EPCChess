using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADSBackend.Data;
using ADSBackend.Models;
using ADSBackend.Models.Identity;
using ADSBackend.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace ADSBackend.Controllers
{
    [Authorize(Roles = "Admin,Advisor")]
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

        public async Task<IActionResult> MatchSetup(int? id)
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

            var homePlayers = match.Games.Where(g => g.HomePlayer != null).Select(g => g.HomePlayerId).ToList();
            var awayPlayers = match.Games.Where(g => g.HomePlayer != null).Select(g => g.AwayPlayerId).ToList();

            ViewBag.HomeStudents = await _context.Player.Where(p => p.PlayerSchoolId == match.HomeSchoolId && !homePlayers.Contains(p.PlayerId))
                                                    .OrderByDescending(p => p.Rating)
                                                    .ToListAsync();

            ViewBag.AwayStudents = await _context.Player.Where(p => p.PlayerSchoolId == match.AwaySchoolId && !awayPlayers.Contains(p.PlayerId))
                                                    .OrderByDescending(p => p.Rating)
                                                    .ToListAsync();

            return View(match);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<String> LockRoster(IFormCollection forms)
        {
            var currentSeason = await SeasonSelector.GetCurrentSeasonId(_context, HttpContext);
            var schoolId = await GetSchoolIdAsync();

            if (schoolId == -1)
                return "SCHOOL NOT FOUND";

            string _id = "", side = "", lockState = "";
            int id = -1;

            if (!forms.ContainsKey("id") &&
                !forms.ContainsKey("side") &&
                !forms.ContainsKey("lockState"))
                return "INVALID FORM";
            

            // Read in form data
            _id = forms["id"];
            side = forms["side"];
            lockState = forms["lockState"];


            // Validate form data
            Int32.TryParse(_id, out id);

            if (side != "home" && side != "away")
                return "UNKNOWN SIDE";

            var match = await _context.Match.FirstOrDefaultAsync(m => m.MatchId == id && (m.HomeSchoolId == schoolId || m.AwaySchoolId == schoolId));

            if (match == null)
            {
                return "NOT FOUND";
            }

            if (side == "home")
                match.HomeRosterLocked = lockState == "true";
            else if (side == "away")
                match.AwayRosterLocked = lockState == "true";

            _context.Update(match);
            await _context.SaveChangesAsync();

            return lockState == "true" ? "LOCKED" : "UNLOCKED";
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<String> UpdateRoster(IFormCollection forms)
        {
            var currentSeason = await SeasonSelector.GetCurrentSeasonId(_context, HttpContext);
            var schoolId = await GetSchoolIdAsync();

            if (schoolId == -1)
                return "SCHOOL NOT FOUND";

            string _id = "", _roster = "", side = "";
            int id = -1;
            List<int?> roster;

            if (!forms.ContainsKey("id") &&
                !forms.ContainsKey("side") &&
                !forms.ContainsKey("roster"))
                return "INVALID FORM";

            // Read in form data
            _id = forms["id"];
            _roster = forms["roster"];
            side = forms["side"];

            // Parse form data
            Int32.TryParse(_id, out id);

            if (side != "home" && side != "away")
                return "UNKNOWN SIDE";

            roster = _roster.Split(',')
                            .Select(i => int.TryParse(i, out int num) ? (int?)num : null)
                            .Where(i => i != null).ToList();

            var match = await _context.Match.Include(m => m.Games)
                                            .FirstOrDefaultAsync(m => m.MatchId == id && (m.HomeSchoolId == schoolId || m.AwaySchoolId == schoolId));

            if (match == null)
            {
                return "NOT FOUND";
            }

            if (match.Games.Count == 0)
            {
                return "ERROR: NO GAMES CREATED FOR THIS MATCH BY BACKEND";
            }

            if (match.AwayRosterLocked && side == "away")
            {
                return "AWAY ROSTER ALREADY LOCKED";
            }

            if (match.HomeRosterLocked && side == "home")
            {
                return "HOME ROSTER ALREADY LOCKED";
            }

            var players = await _context.Player.Where(p => (p.PlayerSchoolId == match.HomeSchoolId || p.PlayerSchoolId == match.AwaySchoolId) && roster.Contains(p.PlayerId)).ToListAsync();

            if (players.Count != roster.Count)
            {
                return "ROSTER CONTAINS MEMBERS NOT IN SELECTED SCHOOL";
            }

            foreach (Game game in match.Games)
            {
                int boardIndex = game.BoardPosition - 1;

                // First we clear anybody that may currently be in the board
                if (side == "home")
                    game.HomePlayerId = null;
                else
                    game.AwayPlayerId = null;

                // Next we see if we can assign a player to the board
                if (boardIndex >= 0 && boardIndex < roster.Count)
                {
                    if (side == "home")
                        game.HomePlayerId = roster[boardIndex];
                    else
                        game.AwayPlayerId = roster[boardIndex];
                }
            }

            _context.Update(match);
            await _context.SaveChangesAsync();

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