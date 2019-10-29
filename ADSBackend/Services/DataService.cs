using ADSBackend.Data;
using ADSBackend.Models;
using ADSBackend.Models.Identity;
using ADSBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ADSBackend.Services
{
    public class DataService : IDataService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HttpContext _httpcontext;

        public DataService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpcontext = httpContextAccessor.HttpContext;
            
        }

        public async Task<RatingEvent> LogRatingEvent (int playerId, int rating, string type = "game", string message = "", bool saveChanges = true, int? gameId = null)
        {
            RatingEvent entry = new RatingEvent
            {
                PlayerId = playerId,
                Rating = rating,
                Type = type,
                Message = message,
                GameId = gameId
            };

            _context.RatingEvent.Add(entry);

            if (saveChanges)
                await _context.SaveChangesAsync();

            return entry;
        }

        public async Task<int> GetCurrentSeasonId()
        {
            var season = await _context.Season.FirstOrDefaultAsync(x => x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now);

            var defaultSeason = season?.SeasonId ?? 1;

            var seasonId = _httpcontext.Session.GetInt32("SeasonId") ?? defaultSeason;

            //var seasonsSelect = new SelectList(seasons, "SeasonId", "Name", seasonId);

            _httpcontext.Session.SetInt32("SeasonId", seasonId);

            return seasonId;
        }

        public async Task<SelectList> GetSeasonSelectList(int currentSeasonId)
        {
            var seasons = await _context.Season.Select(x => x)
                                         .OrderByDescending(x => x.StartDate)
                                         .ToListAsync();

            return new SelectList(seasons, "SeasonId", "Name", currentSeasonId);
        }

        public async Task<int> GetSchoolIdAsync(ClaimsPrincipal User)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return -1;

            return user.SchoolId;
        }

        // Returns Match matching matchid id and seasonId and optionally matches a schoolId
        public async Task<Match> GetMatchAsync(int? id, int seasonId, int schoolId)
        {
            if (id == null)
                return null;

            var match = await _context.Match.Include(m => m.HomeSchool).ThenInclude(m => m.Season)
                .Include(m => m.AwaySchool).ThenInclude(m => m.Season)
                .Include(m => m.Games).ThenInclude(g => g.HomePlayer)
                .Include(m => m.Games).ThenInclude(g => g.AwayPlayer)
                .Where(m => m.MatchId == id && m.HomeSchool.SeasonId == seasonId && 
                            (m.HomeSchoolId == schoolId || m.AwaySchoolId == schoolId) )
                .FirstOrDefaultAsync();

            return match;
        }

        public async Task<List<Match>> GetUpcomingMatchesAsync (int seasonId, int schoolId, int count = 4)
        {
            return await _context.Match.Where(m => m.MatchDate >= DateTime.Now && (m.HomeSchoolId == schoolId || m.AwaySchoolId == schoolId))
                                                     .OrderBy(m => m.MatchDate)
                                                     .Take(count)
                                                     .ToListAsync();
        }

        public async Task<List<Division>> GetDivisionStandingsAsync (int seasonId)
        {
            var divisions = await _context.Division.OrderBy(d => d.Name).ToListAsync();

            Dictionary<int, School> scores = new Dictionary<int, School>();  // Maps schoolId to School

            var schools = await _context.School.Where(s => s.SeasonId == seasonId)
                                               .OrderBy(s => s.Name)
                                               .ToListAsync();

            foreach (var school in schools)
            {
                scores.Add(school.SchoolId, school);
            }

            var matches = await _context.Match.Include(m => m.HomeSchool)
                                              .Include(m => m.AwaySchool)
                                              .Where(m => m.HomeSchool.SeasonId == seasonId && m.AwaySchool.SeasonId == seasonId && m.Completed)
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

            foreach (Division d in divisions)
            {
                d.Schools = schools.Where(s => s.DivisionId == d.DivisionId)
                                   .OrderByDescending(s => s.Points)
                                   .ThenBy(s => s.Name)
                                   .ToList();
            }

            return divisions;
        }
    }
}
