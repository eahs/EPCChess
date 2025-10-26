
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADSBackend.Data;
using ADSBackend.Models;
using ADSBackend.Models.Identity;
using ADSBackend.Services;
using ADSBackend.Util;
using LichessApi;
using LichessApi.Web.Api.BulkPairings.Request;
using LichessApi.Web.Api.Games;
using LichessApi.Web.Entities.Enum;
using LichessApi.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Game = ADSBackend.Models.Game;

namespace ADSBackend.Controllers
{
    /// <summary>
    /// Controller for managing school matches.
    /// </summary>
    [Authorize(Roles = "Admin,Advisor")]
    public class SchoolMatchesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DataService _dataService;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchoolMatchesController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="dataService">The data service.</param>
        /// <param name="configuration">The configuration.</param>
        public SchoolMatchesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, DataService dataService, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _dataService = dataService;
            _configuration = configuration;
        }

        /// <summary>
        /// Updates the win/loss/draw records for all players.
        /// </summary>
        /// <returns>A string indicating the result of the operation.</returns>
        public async Task<string> UpdatePlayers()
        {
            await _dataService.UpdatePlayerRecords();

            return "OK";
        }

        /// <summary>
        /// Displays a list of matches for the advisor's school.
        /// </summary>
        /// <returns>The index view with a list of matches.</returns>
        public async Task<IActionResult> Index()
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();
            var schoolId = await _dataService.GetSchoolIdAsync(User, currentSeason);

            if (schoolId == -1)
            {
                return RedirectToAction("Index", "Admin");
            }

            var matches = await _context.Match.Include(m => m.HomeSchool).ThenInclude(m => m.Season)
                                              .Include(m => m.AwaySchool).ThenInclude(m => m.Season)
                                              .Where(m => m.HomeSchool.SeasonId == currentSeason && (m.HomeSchoolId == schoolId || m.AwaySchoolId == schoolId))
                                              .OrderBy(m => m.MatchDate)
                                              .ToListAsync();

            ViewBag.School = await _context.School.FirstOrDefaultAsync(m => m.SchoolId == schoolId);

            return View(matches);
        }

        /// <summary>
        /// Displays the management page for a specific match.
        /// </summary>
        /// <param name="id">The ID of the match.</param>
        /// <returns>The manage view for the match.</returns>
        public async Task<IActionResult> Manage(int? id)
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();
            var schoolId = await _dataService.GetSchoolIdAsync(User, currentSeason);

            if (schoolId == -1)
            {
                return RedirectToAction("Index", "Admin");
            }

            if (id == null)
            {
                return NotFound();
            }

            var match = await _dataService.GetMatchAsync(id, currentSeason, schoolId);

            if (match == null)
            {
                return NotFound();
            }

            if (match.Games == null || match.Games.Count != 12)
            {
                for (int board = 1; board <= 12; board++)
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


        /// <summary>
        /// Displays the setup page for a specific match.
        /// </summary>
        /// <param name="id">The ID of the match.</param>
        /// <returns>The match setup view.</returns>
        public async Task<IActionResult> MatchSetup(int? id)
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();
            var schoolId = await _dataService.GetSchoolIdAsync(User, currentSeason);

            if (schoolId == -1)
            {
                return RedirectToAction("Index", "Admin");
            }

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
            var awayPlayers = match.Games.Where(g => g.AwayPlayer != null).Select(g => g.AwayPlayerId).ToList();

            var homeStudents = await _context.Player.Where(p => p.PlayerSchoolId == match.HomeSchoolId && !homePlayers.Contains(p.PlayerId))
                .OrderByDescending(p => p.Rating)
                .ToListAsync();

            var awayStudents = await _context.Player.Where(p => p.PlayerSchoolId == match.AwaySchoolId && !awayPlayers.Contains(p.PlayerId))
                .OrderByDescending(p => p.Rating)
                .ToListAsync();

            ViewBag.HomeSkippedStudents = new List<Player>();
            ViewBag.AwaySkippedStudents = new List<Player>();

            if (match.IsVirtual)
            {
                ViewBag.HomeSkippedStudents = homeStudents.Where(p => p.UserId == null).ToList();
                ViewBag.AwaySkippedStudents = awayStudents.Where(p => p.UserId == null).ToList(); 

                homeStudents = homeStudents.Where(p => p.UserId != null).ToList();
                awayStudents = awayStudents.Where(p => p.UserId != null).ToList();
            }

            ViewBag.HomeStudents = homeStudents;
            ViewBag.AwayStudents = awayStudents;

            return View(match);
        }

        private string JsonStatus(string message)
        {
            return JsonConvert.SerializeObject(new {Status = message});
        }

        /// <summary>
        /// Converts a game result enum to points won.
        /// </summary>
        /// <param name="result">The result of the game.</param>
        /// <param name="playerNumber">The player number (1 or 2).</param>
        /// <returns>The points won (1 for a win, 0.5 for a draw, 0 for a loss).</returns>
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


        /// <summary>
        /// Begins a match.
        /// </summary>
        /// <param name="forms">The form collection from the request.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<String> BeginMatch(IFormCollection forms)
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();
            var schoolId = await _dataService.GetSchoolIdAsync(User, currentSeason);
            var user = await _dataService.GetUserAsync(User);

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

            var lichessAuthNSection = _configuration.GetSection("Authentication:Lichess");

            var accessToken = lichessAuthNSection["ApiToken"];

			if (!string.IsNullOrEmpty(accessToken))
            {
	            LichessApi.LichessApiClient client = new LichessApiClient(accessToken);

	            var players = new List<string>();

	            foreach (Game g in match.Games)
	            {
		            if (g.HomePlayer != null && g.AwayPlayer != null)
		            {
			            // If odd board home player is black
			            var homeBlack = g.BoardPosition % 2 == 1;

			            // If JV it's flipped
			            if (g.BoardPosition > 7)
				            homeBlack = !homeBlack;

			            players.Add(homeBlack
				            ? $"{g.AwayPlayer.User.AccessToken}:{g.HomePlayer.User.AccessToken}"
				            : $"{g.HomePlayer.User.AccessToken}:{g.AwayPlayer.User.AccessToken}");
		            }
	            }

	            if (players.Count > 0)
	            {
		            var request = new BulkPairingRequest
		            {
			            Players = String.Join(",", players),
			            ClockLimit = match.ClockTimeLimit,
			            ClockIncrement = match.ClockIncrement,
			            Variant = GameVariant.Standard,
			            StartClocksAt = null,
                        PairAt = DateTime.UtcNow,
			            Rated = true,
			            Message = "Your EPC team game with {opponent} is ready: {game}."
		            };

		            var response = await client.BulkPairings.CreateBulkPairing(request);

		            if (response?.Games?.Count > 0)
		            {
			            foreach (var bulkGame in response.Games)
			            {
				            var game = match.Games.Find(x => x.HomePlayer.User.LichessId == bulkGame.BlackPlayer ||
				                                             x.HomePlayer.User.LichessId == bulkGame.WhitePlayer);

				            if (game != null)
				            {
					            game.ChallengeId = bulkGame.Id;
					            game.ChallengeUrl = $"https://lichess.org/{bulkGame.Id}";
					            game.IsStarted = true;
					            game.CurrentFen = request.Fen;

					            _context.Update(game);
				            }
			            }
		            }
	            }
			}

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

        /// <summary>
        /// Ends a match.
        /// </summary>
        /// <param name="forms">The form collection from the request.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<String> EndMatch(IFormCollection forms)
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();
            var schoolId = await _dataService.GetSchoolIdAsync(User, currentSeason);

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

            // Now update all the player win, loss, draw records
            await _dataService.UpdatePlayerRecords();

            return "OK";
        }

        /// <summary>
        /// Reports the result of a game.
        /// </summary>
        /// <param name="forms">The form collection from the request.</param>
        /// <returns>A JSON string with the status and updated game information.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<String> ReportResult(IFormCollection forms)
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();
            var schoolId = await _dataService.GetSchoolIdAsync(User, currentSeason);

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
            if (match.Completed) return JsonStatus("MATCH HAS BEEN COMPLETED");


            var game = await _context.Game.Include(g => g.HomePlayer)
                                          .Include(g => g.AwayPlayer)
                                          .FirstOrDefaultAsync(g => g.MatchId == matchid && g.GameId == gameid);

            if (game == null) return JsonStatus("GAME NOT FOUND");

            // Result: 0 = Draw, 1 = Home Win, 2 = Away Win, 3 = Reset

            gameResult = (GameResult)result;

            await _dataService.UpdateAndLogRatingCalculations(game, gameResult);

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


        /// <summary>
        /// Locks or unlocks a team's roster for a match.
        /// </summary>
        /// <param name="forms">The form collection from the request.</param>
        /// <returns>A string indicating the new lock state.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<String> LockRoster(IFormCollection forms)
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();
            var schoolId = await _dataService.GetSchoolIdAsync(User, currentSeason);

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


        /// <summary>
        /// Updates the roster for a team in a match.
        /// </summary>
        /// <param name="forms">The form collection from the request.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<String> UpdateRoster(IFormCollection forms)
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();
            var schoolId = await _dataService.GetSchoolIdAsync(User, currentSeason);

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