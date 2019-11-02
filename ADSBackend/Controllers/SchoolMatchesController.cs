using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADSBackend.Data;
using ADSBackend.Models;
using ADSBackend.Models.Identity;
using ADSBackend.Services;
using ADSBackend.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace ADSBackend.Controllers
{
    [Authorize(Roles = "Admin,Advisor")]
    public class SchoolMatchesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DataService _dataService;

        public SchoolMatchesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, DataService dataService)
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

        public async Task<IActionResult> Manage(int? id)
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

            if (match.Games == null || match.Games.Count != 10)
            {
                for (int board = 1; board <= 10; board++)
                {
                    if (match.Games != null)
                    {
                        // Is there a game with this boardposition already?
                        var gm = match.Games.Where(gp => gp.BoardPosition == board).FirstOrDefault();
                        if (gm != null) 
                            continue;
                    }

                    Game g = new Game
                    {
                        MatchId = (int) id,
                        BoardPosition = board
                    };
                    _context.Add(g);

                }
                await _context.SaveChangesAsync();

                match = await _dataService.GetMatchAsync(id, currentSeason, schoolId);
            }

            return View(match);
        }


        public async Task<IActionResult> MatchSetup(int? id)
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

            if (match.MatchStarted)
            {
                return RedirectToAction("Manage", "SchoolMatches", new { id }); 
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

        private string JsonStatus(string message)
        {
            return JsonConvert.SerializeObject(new {Status = message});
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


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<String> BeginMatch(IFormCollection forms)
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();
            var schoolId = await _dataService.GetSchoolIdAsync(User);

            if (schoolId == -1)
                return "SCHOOL NOT FOUND";

            string _id = "";
            int id = -1;

            if (!forms.ContainsKey("id"))
                return "INVALID FORM";


            // Read in form data
            _id = forms["id"];

            // Validate form data
            Int32.TryParse(_id, out id);

            var match = await _dataService.GetMatchAsync(id, currentSeason, schoolId);

            if (match == null)
            {
                return "NOT FOUND";
            }

            if (!match.HomeRosterLocked || !match.AwayRosterLocked)
            {
                return "BOTH ROSTERS NOT YET LOCKED";
            }

            if (match.MatchStarted)
            {
                return "RELOAD";
            }

            match.MatchStarted = true;
            match.MatchStartTime = DateTime.Now;
            
            foreach (Game g in match.Games)
            {
                bool update = false;

                if (g.HomePlayer != null)
                {
                    g.HomePlayerRatingBefore = g.HomePlayer.Rating;
                    update = true;
                }

                if (g.AwayPlayer != null)
                {
                    g.AwayPlayerRatingBefore = g.AwayPlayer.Rating;
                    update = true;
                }

                // Quick sanity check because we can't count boards past 7 in a match if either isn't seated
                if ((g.HomePlayer == null || g.AwayPlayer == null) && g.BoardPosition > 7)
                    update = false;

                if (update)
                {
                    _context.Update(g);
                }
            }

            _context.Update(match);
            await _context.SaveChangesAsync();

            return "OK";
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<String> EndMatch(IFormCollection forms)
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();
            var schoolId = await _dataService.GetSchoolIdAsync(User);

            if (schoolId == -1)
                return "SCHOOL NOT FOUND";

            string _id = "";
            int id = -1;

            if (!forms.ContainsKey("id"))
                return "INVALID FORM";


            // Read in form data
            _id = forms["id"];

            // Validate form data
            Int32.TryParse(_id, out id);

            var match = await _dataService.GetMatchAsync(id, currentSeason, schoolId);

            if (match == null)
            {
                return "NOT FOUND";
            }

            if (!match.HomeRosterLocked || !match.AwayRosterLocked)
            {
                return "BOTH ROSTERS NOT YET LOCKED";
            }

            if (!match.MatchStarted)
            {
                return "MATCH NOT STARTED";
            }

            if (match.Completed)
            {
                return "MATCH ALREADY COMPLETED";
            }

            int gamesScheduled = 0, gamesPlayed = 0;
            double homePoints = 0, awayPoints = 0;

            foreach (Game g in match.Games)
            {
                if (g.HomePlayer != null && g.AwayPlayer != null)
                {
                    gamesScheduled++;
                    if (g.HomePoints + g.AwayPoints != 0)
                    {
                        gamesPlayed++;

                        if (g.BoardPosition <= 7)
                        {
                            homePoints += g.HomePoints;
                            awayPoints += g.AwayPoints;
                        }
                    }
                }
                else if (g.HomePlayer != null && g.BoardPosition <= 7)
                {
                // Forfeit by awayplayer
                    homePoints += 1;
                    g.Completed = true;
                    g.CompletedDate = DateTime.Now;
                    g.HomePoints = 1;
                    g.AwayPoints = 0;
                    g.HomePlayerRatingAfter = g.HomePlayerRatingBefore + 16;
                    g.HomePlayer.Rating += 16;

                    await _dataService.LogRatingEvent(g.HomePlayer.PlayerId, g.HomePlayer.Rating, "game", "Win by forfeit", false, g.GameId);

                    _context.Update(g);
                    _context.Update(g.HomePlayer);
                }
                else if (g.AwayPlayer != null && g.BoardPosition <= 7)
                {
                    // Forfeit by homeplayer
                    awayPoints += 1;
                    g.Completed = true;
                    g.CompletedDate = DateTime.Now;
                    g.HomePoints = 0;
                    g.AwayPoints = 1;
                    g.AwayPlayerRatingAfter = g.AwayPlayerRatingBefore + 16;
                    g.AwayPlayer.Rating += 16;

                    await _dataService.LogRatingEvent(g.AwayPlayer.PlayerId, g.AwayPlayer.Rating, "game", "Win by forfeit", false, g.GameId);

                    _context.Update(g);
                    _context.Update(g.AwayPlayer);
                }
            }

            if (gamesPlayed != gamesScheduled)
            {
                return $"Only {gamesPlayed} out of {gamesScheduled} games have had results submitted.  Please submit all game results before concluding match.";
            }

            match.Completed = true;
            match.HomePoints = homePoints;
            match.AwayPoints = awayPoints;

            _context.Update(match);
            await _context.SaveChangesAsync();

            return "OK";
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<String> ReportResult(IFormCollection forms)
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();
            var schoolId = await _dataService.GetSchoolIdAsync(User);

            if (schoolId == -1)
                return JsonStatus("SCHOOL NOT FOUND");

            string _gameid = "", _matchid = "", _result = "";
            int gameid = -1, matchid = -1, result = -1;
            GameResult gameResult;

            if (!forms.ContainsKey("gameid") &&
                !forms.ContainsKey("matchid") &&
                !forms.ContainsKey("result"))
                return JsonStatus("INVALID FORM");

            // Read in form data
            _gameid = forms["gameid"];
            _matchid = forms["matchid"];
            _result = forms["result"];

            // Validate form data
            Int32.TryParse(_gameid, out gameid);
            Int32.TryParse(_matchid, out matchid);
            Int32.TryParse(_result, out result);

            if (result < 0 || result > 3) return JsonStatus("Invalid result code");

            var match = await _context.Match.FirstOrDefaultAsync(m => m.MatchId == matchid && (m.HomeSchoolId == schoolId || m.AwaySchoolId == schoolId));

            if (match == null) return JsonStatus("MATCH NOT FOUND");
            if (!match.MatchStarted) return JsonStatus("MATCH NOT STARTED");


            var game = await _context.Game.Include(g => g.HomePlayer)
                                          .Include(g => g.AwayPlayer)
                                          .FirstOrDefaultAsync(g => g.MatchId == matchid && g.GameId == gameid);

            if (game == null) return JsonStatus("GAME NOT FOUND");

            // Result: 0 = Draw, 1 = Home Win, 2 = Away Win, 3 = Reset

            gameResult = (GameResult)result;

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

                    await _dataService.LogRatingEvent(game.HomePlayer.PlayerId, game.HomePlayer.Rating, "adjustment", "Game reset by advisor", false, game.GameId);
                    await _dataService.LogRatingEvent(game.AwayPlayer.PlayerId, game.AwayPlayer.Rating, "adjustment", "Game reset by advisor", false, game.GameId);

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
                        await _dataService.LogRatingEvent(game.HomePlayer.PlayerId, game.HomePlayer.Rating, "adjustment", "Player rating changed after match started", false, game.GameId);
                    
                    if (game.AwayPlayerRatingBefore != game.AwayPlayer.Rating)
                        await _dataService.LogRatingEvent(game.AwayPlayer.PlayerId, game.AwayPlayer.Rating, "adjustment", "Player rating changed after match started", false, game.GameId);

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

                game.CompletedDate = DateTime.Now;

                _context.Update(game.HomePlayer);
                _context.Update(game.AwayPlayer);

                await _dataService.LogRatingEvent(game.HomePlayer.PlayerId, game.HomePlayer.Rating, "game", "End of game result", false, game.GameId);
                await _dataService.LogRatingEvent(game.AwayPlayer.PlayerId, game.AwayPlayer.Rating, "game", "End of game result", false, game.GameId);

            }

            _context.Update(game);
            await _context.SaveChangesAsync();

            var returnStatus = new
            {
                Status = "OK",
                game.GameId,
                game.HomePoints,
                game.AwayPoints,
                game.HomePlayerFullName,
                game.AwayPlayerFullName,
                game.HomePlayerGameRating,
                game.AwayPlayerGameRating
            };

            return JsonConvert.SerializeObject(returnStatus);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<String> LockRoster(IFormCollection forms)
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();
            var schoolId = await _dataService.GetSchoolIdAsync(User);

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
            var currentSeason = await _dataService.GetCurrentSeasonId();
            var schoolId = await _dataService.GetSchoolIdAsync(User);

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

    }
}