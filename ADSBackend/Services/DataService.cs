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
        internal class PlayerRecord
        {
            public int Wins { get; set; } = 0;
            public int Losses { get; set; } = 0;
            public int Draws { get; set; } = 0;
        }

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HttpContext _httpcontext;

        public DataService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpcontext = httpContextAccessor.HttpContext;
            
        }

        public async Task SyncExternalPlayer(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var player = await _context.Player.FirstOrDefaultAsync(p => p.UserId == userId);

            bool isPlayer = await _userManager.IsInRoleAsync(user, "Player");

            if (player == null && isPlayer)
            {
                player = new Player
                {
                    UserId = userId,
                    FirstName = user.FirstName ?? "",
                    LastName = user.LastName ?? "",
                    PlayerSchoolId = user.SchoolId,
                    Rating = 1000
                };

                _context.Player.Add(player);
                await _context.SaveChangesAsync();
            }
            else
            {
                if (!isPlayer)
                {
                    if (player != null)
                        _context.Player.Remove(player);
                }
                else
                {
                    player.FirstName = user.FirstName;
                    player.LastName = user.LastName;
                    player.PlayerSchoolId = user.SchoolId;

                    _context.Player.Update(player);
                }

                await _context.SaveChangesAsync();
            }

        }

        public async Task UpdatePlayerRecords ()
        {
            var currentSeason = await GetCurrentSeasonId();
            var games = await _context.Game.Include(g => g.Match).ThenInclude(m => m.HomeSchool)
                                           .Where(g => g.Completed && g.Match.HomeSchool.SeasonId == currentSeason)
                                           .ToListAsync();

            Dictionary<int, PlayerRecord> records = new Dictionary<int, PlayerRecord>();

            foreach (var game in games)
            {
                if (game.HomePlayerId != null)
                {
                    int playerId = (int)game.HomePlayerId;
                    if (!records.ContainsKey(playerId))
                    {
                        records.Add(playerId, new PlayerRecord());
                    }
                    if (game.HomePoints == 1) records[playerId].Wins++;
                    if (game.HomePoints == 0) records[playerId].Losses++;
                    if (game.HomePoints == 0.5) records[playerId].Draws++;
                }

                if (game.AwayPlayerId != null)
                {
                    int playerId = (int)game.AwayPlayerId;
                    if (!records.ContainsKey(playerId))
                    {
                        records.Add(playerId, new PlayerRecord());
                    }
                    if (game.AwayPoints == 1) records[playerId].Wins++;
                    if (game.AwayPoints == 0) records[playerId].Losses++;
                    if (game.AwayPoints == 0.5) records[playerId].Draws++;
                }

            }

            var players = await _context.Player.Include(p => p.PlayerSchool)
                                               .Where(p => p.PlayerSchool.SeasonId == currentSeason)
                                               .ToListAsync();

            foreach (var player in players)
            {
                if (records.ContainsKey(player.PlayerId))
                {
                    player.Wins = records[player.PlayerId].Wins;
                    player.Losses = records[player.PlayerId].Losses;
                    player.Draws = records[player.PlayerId].Draws;

                    _context.Player.Update(player);
                }
            }
            await _context.SaveChangesAsync();
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
            return await _context.Match.Include(m => m.HomeSchool)
                                                     .Where(m => m.HomeSchool.SeasonId == seasonId && 
                                                                 m.MatchDate >= DateTime.Now &&
                                                                 (m.HomeSchoolId == schoolId || m.AwaySchoolId == schoolId))
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
