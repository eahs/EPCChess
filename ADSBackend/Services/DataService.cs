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
using ADSBackend.Util;

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
        private readonly SignInManager<ApplicationUser> _signInManager;

        public DataService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpcontext = httpContextAccessor.HttpContext;
            _signInManager = signInManager;
        }

        public async Task UpdateAndLogRatingCalculations(Game game, GameResult gameResult)
        {
            if (gameResult == GameResult.Reset)
            {
                game.HomePoints = 0;
                game.AwayPoints = 0;
                game.HomePlayerRatingAfter = 0;
                game.AwayPlayerRatingAfter = 0;
                game.Completed = false;

                if (game.HomePoints + game.AwayPoints != 0)
                {
                    game.HomePlayer.Rating = game.HomePlayerRatingBefore;
                    game.AwayPlayer.Rating = game.AwayPlayerRatingBefore;

                    _context.Update(game.HomePlayer);
                    _context.Update(game.AwayPlayer);

                    await LogRatingEvent(game.HomePlayer.PlayerId, game.HomePlayer.Rating, "adjustment", "Game reset by advisor", false, game.GameId);
                    await LogRatingEvent(game.AwayPlayer.PlayerId, game.AwayPlayer.Rating, "adjustment", "Game reset by advisor", false, game.GameId);

                }

            }
            else
            {
                int homeRating, awayRating;

                // Update player's rating in case it was adjusted before this game was submitted
                // This only happens if there are two matches running at the same time 
                if (game.HomePoints + game.AwayPoints == 0)
                {
                    if (game.HomePlayerRatingBefore != game.HomePlayer.Rating)
                        await LogRatingEvent(game.HomePlayer.PlayerId, game.HomePlayer.Rating, "adjustment", "Player rating changed after match started", false, game.GameId);

                    if (game.AwayPlayerRatingBefore != game.AwayPlayer.Rating)
                        await LogRatingEvent(game.AwayPlayer.PlayerId, game.AwayPlayer.Rating, "adjustment", "Player rating changed after match started", false, game.GameId);

                    game.HomePlayerRatingBefore = game.HomePlayer.Rating;
                    game.AwayPlayerRatingBefore = game.AwayPlayer.Rating;
                }

                RatingCalculator.Current.CalculateNewRating(game.HomePlayerRatingBefore, game.AwayPlayerRatingBefore,
                    gameResult, out homeRating, out awayRating);

                game.HomePoints = ConvertPointsWon(gameResult, 1);
                game.AwayPoints = ConvertPointsWon(gameResult, 2); ;
                game.HomePlayerRatingAfter = homeRating;
                game.AwayPlayerRatingAfter = awayRating;

                game.HomePlayer.Rating = homeRating;
                game.AwayPlayer.Rating = awayRating;

                game.Completed = true;
                game.CompletedDate = DateTime.Now;

                _context.Update(game.HomePlayer);
                _context.Update(game.AwayPlayer);

                await LogRatingEvent(game.HomePlayer.PlayerId, game.HomePlayer.Rating, "game", "End of game result", false, game.GameId);
                await LogRatingEvent(game.AwayPlayer.PlayerId, game.AwayPlayer.Rating, "game", "End of game result", false, game.GameId);

            }
        }

        public async Task SyncExternalPlayer(int userId)
        {
            var user = await GetUserByIdAsync(userId);

            int currentSchoolId = user.Schools.Select(s => s.SchoolId).Max();

            var player = await _context.Player.OrderByDescending(p => p.PlayerId).FirstOrDefaultAsync(p => p.UserId == userId && p.PlayerSchoolId == currentSchoolId);
            
            var info = await _userManager.GetLoginsAsync(user);

            if (info is not null)
            {
                var loginrec = info.FirstOrDefault(ul => ul.LoginProvider.Equals("Lichess"));

                if (loginrec is not null)
                {
                    user.LichessId = loginrec.ProviderKey;

                    await _userManager.UpdateAsync(user);
                }
            }

            bool isPlayer = await _userManager.IsInRoleAsync(user, "Player");

            if (player == null && isPlayer)
            {
                player = new Player
                {
                    UserId = userId,
                    FirstName = user.FirstName ?? "",
                    LastName = user.LastName ?? "",
                    PlayerSchoolId = currentSchoolId,
                    Rating = 1000
                };

                _context.Player.Add(player);
                await _context.SaveChangesAsync();
            }
            else
            {
                if (!isPlayer)
                {
                    // We don't want to remove them if they aren't a player for a current season for past seasons
                    // if (player != null)
                    //    _context.Player.Remove(player);
                }
                else
                {
                    player.FirstName = user.FirstName;
                    player.LastName = user.LastName;
                    player.PlayerSchoolId = currentSchoolId;

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

        /// <summary>
        /// Properly retrieves a user with extension navigation properties
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public async Task<ApplicationUser> GetUserAsync(ClaimsPrincipal User)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            return await GetUserByIdAsync(userId);
        }

        public async Task<ApplicationUser> GetUserByIdAsync(int userId)
        {
            return await _context.Users.Include(x => x.Schools).ThenInclude(s => s.School).SingleOrDefaultAsync(x => x.Id == userId);
            
        }

        public async Task RemoveUserFromSchool(ClaimsPrincipal User, int schoolId)
        {
            var user = await GetUserAsync(User);
            await RemoveUserFromSchool(user, schoolId);
        }

        public async Task RemoveUserFromSchool(ApplicationUser User, int schoolId)
        {
            var userSchool =
                await _context.UserSchool.FirstOrDefaultAsync(x => x.UserId == User.Id && x.SchoolId == schoolId);

            if (userSchool is not null)
            {
                _context.UserSchool.Remove(userSchool);

                await _context.SaveChangesAsync();
            }
        }

        public async Task AddUserToSchoolAsync(ClaimsPrincipal User, int schoolId)
        {
            var user = await GetUserAsync(User);
            await AddUserToSchoolAsync(user, schoolId);
        }

        public async Task AddUserToSchoolAsync(ApplicationUser User, int schoolId)
        {
            if (User is null) return;

            var exists = User.Schools?.FirstOrDefault(s => s.SchoolId == schoolId);

            if (exists is null)
            {
                var school = new UserSchool
                {
                    UserId = User.Id,
                    SchoolId = schoolId
                };
                _context.UserSchool.Add(school);
                await _context.SaveChangesAsync();

                User.Schools ??= new List<UserSchool>();
                User.Schools.Add(school);
            }

            
        }

        public async Task<int> GetSchoolIdAsync(ClaimsPrincipal User, int seasonId)
        {
//            var user = await _userManager.GetUserAsync(User);
            var user = await GetUserAsync(User);

            if (user == null)
                return -1;

            var school = user.Schools.Where(s => s.School.SeasonId == seasonId).OrderByDescending(s => s.School.SeasonId).FirstOrDefault();

            if (school is null)
                return -1;

            return school.SchoolId;
        }

        // Returns Match matching matchid id and seasonId and optionally matches a schoolId
        public async Task<Match> GetMatchAsync(int? id, int seasonId, int schoolId = -1)
        {
            if (id == null)
                return null;

            var match = await _context.Match.Include(m => m.HomeSchool).ThenInclude(m => m.Season)
                .Include(m => m.AwaySchool).ThenInclude(m => m.Season)
                .Include(m => m.Games).ThenInclude(g => g.HomePlayer).ThenInclude(p => p.User)
                .Include(m => m.Games).ThenInclude(g => g.AwayPlayer).ThenInclude(p => p.User)
                .Where(m => m.MatchId == id && m.HomeSchool.SeasonId == seasonId && 
                            (schoolId == -1 || m.HomeSchoolId == schoolId || m.AwaySchoolId == schoolId) )
                .FirstOrDefaultAsync();

            return match;
        }

        public async Task<List<Match>> GetUpcomingMatchesAsync (int seasonId, int schoolId, int count = 4)
        {
            return await _context.Match.Include(m => m.HomeSchool)
                                                     .Where(m => m.HomeSchool.SeasonId == seasonId && 
                                                                 m.MatchDate.Date >= DateTime.Now.Date &&
                                                                 (m.HomeSchoolId == schoolId || m.AwaySchoolId == schoolId))
                                                     .OrderBy(m => m.MatchDate)
                                                     .Take(count)
                                                     .ToListAsync();
        }

        public async Task<List<Division>> GetDivisionStandingsAsync (int seasonId)
        {
            var divisions = await _context.Division.Where(d => d.SeasonId == seasonId).OrderBy(d => d.Name).ToListAsync();

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


        public double ConvertPointsWon(GameResult result, int playerNumber)
        {
            if (result == GameResult.Draw) return 0.5;

            if (playerNumber == 1)
            {
                if (result == GameResult.Player1Wins) return 1;
            }
            else
            {
                if (result == GameResult.Player2Wins) return 1;
            }

            return 0;
        }
    }
}
