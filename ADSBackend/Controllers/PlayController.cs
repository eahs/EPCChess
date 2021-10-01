using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADSBackend.Data;
using ADSBackend.Hubs;
using ADSBackend.Models;
using ADSBackend.Models.Identity;
using ADSBackend.Models.PlayhubViewModels;
using ADSBackend.Models.PlayViewModels;
using ADSBackend.Services;
using LichessApi;
using LichessApi.Web.Api.Challenges.Request;
using LichessApi.Web.Entities.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;

namespace ADSBackend.Controllers
{
    [Authorize(Roles = "Admin,Advisor,Player")]
    public class PlayController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly DataService _dataService;
        private readonly ITokenRefresher _tokenRefresher;
        private readonly IHubContext<GameHub> _hubContext;

        public PlayController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, DataService dataService, ITokenRefresher tokenRefresher, IHubContext<GameHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _dataService = dataService;
            _tokenRefresher = tokenRefresher;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var currentSeason = await _dataService.GetCurrentSeasonId();
            var schoolId = await _dataService.GetSchoolIdAsync(User, currentSeason);

            if (schoolId == -1)
                return RedirectToAction("Index", "Admin");

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
            var schoolId = await _dataService.GetSchoolIdAsync(User, currentSeason);
            var user = await _dataService.GetUserAsync(User);

            if (schoolId == -1)
                return RedirectToAction("Index", "Admin");

            if (id == null)
            {
                return NotFound();
            }

            var match = await _dataService.GetMatchAsync(id, currentSeason, schoolId);

            if (match == null)
            {
                return NotFound();
            }

            var chat = await _context.MatchChat.Where(m => m.MatchId == match.MatchId)
                .Include(m => m.Match)
                .Include(m => m.User)
                .OrderBy(m => m.MessageDate)
                .ToListAsync();

            MatchViewModel viewmodel = new MatchViewModel
            {
                Match = match,
                ViewingUser = user,
                Chat = chat ?? new List<MatchChat>()
            };

            return View(viewmodel);
        }

        public async Task<IActionResult> IssueChallenge(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var user = await _dataService.GetUserAsync(User);
            var game = await _context.Game.Include(g => g.HomePlayer.User)
                .Include(g => g.AwayPlayer.User)
                .Include(g => g.Match)
                .FirstOrDefaultAsync(g => g.GameId == id);

            if (game.HomePlayer == null || game.AwayPlayer == null || game.HomePlayer.User.Id != user.Id)
            {
                return NotFound();
            }

            var awayUser = await _userManager.FindByIdAsync(game.AwayPlayer.UserId + "");

            bool userRefreshed = false, awayRefreshed = false;

            userRefreshed = await _tokenRefresher.RefreshTokens(user, false);
            awayRefreshed = await _tokenRefresher.RefreshTokens(awayUser, false);
            
            if (!userRefreshed)
            {
                Log.Information("User access token expired");

                await _signInManager.SignOutAsync();
                return RedirectToAction(nameof(AdminController.Index), "Home");
            }

            GameJson gameJson = new GameJson
            {
                GameId = game.GameId.ToString(),
                ChallengeId = game.ChallengeId,
                Fen = game.CurrentFen,
                ChallengeUrl = game.ChallengeUrl,
                MatchId = game.MatchId,
                Moves = new List<string>(),
                Completed = false,
                Result = "",
                BlackPlayerId = game.BoardPosition % 2 == 1 ? game.HomePlayer.User.LichessId : game.AwayPlayer.User.LichessId,
                WhitePlayerId = game.BoardPosition % 2 == 0 ? game.HomePlayer.User.LichessId : game.AwayPlayer.User.LichessId,
                HomePlayerRating = game.Completed ? $"{game.HomePlayerRatingBefore} -> {game.HomePlayerRatingAfter}" : $"{game.HomePlayerRatingAfter}",
                AwayPlayerRating = game.Completed ? $"{game.AwayPlayerRatingBefore} -> {game.AwayPlayerRatingAfter}" : $"{game.AwayPlayerRatingAfter}",
                HomePoints = game.HomePoints.ToString(),
                AwayPoints = game.AwayPoints.ToString()
            };

            // If JV, colors are swapped
            if (game.BoardPosition > 7)
            {
                string temp = gameJson.BlackPlayerId;
                gameJson.BlackPlayerId = gameJson.WhitePlayerId;
                gameJson.WhitePlayerId = temp;
            }

            bool challengeCreated = false;

            LichessApi.LichessApiClient client = new LichessApiClient(user.AccessToken);

            if (awayRefreshed)
            {
                CreateGameRequest request = new CreateGameRequest
                {
                    Rated = true,
                    ClockLimit = game.Match.ClockTimeLimit,
                    ClockIncrement = game.Match.ClockIncrement,
                    Color = game.BoardPosition % 2 == 0 ? Color.White : Color.Black,
                    Variant = GameVariant.Standard,
                    Fen = game.CurrentFen,
                    Message = "Your EPC team game with {opponent} is ready: {game}."
                };

                // Jv board colors are swapped
                if (game.BoardPosition > 7)
                {
                    if (request.Color == Color.White)
                    {
                        request.Color = Color.Black;
                    }
                    else
                    {
                        request.Color = Color.White;
                    }
                }

                try
                {
                    var response = await client.Challenges.CreateGame(awayUser.LichessId, awayUser.AccessToken, request);

                    game.CurrentFen = response.Game.InitialFen;
                    game.ChallengeUrl = response.Game.Url;
                    game.ChallengeId = response.Game.Id;
                    game.ChallengeJson = JsonConvert.SerializeObject(response);
                    game.IsStarted = true;

                    gameJson.Status = response.Game.Status.Name;
                    gameJson.IsStarted = true;

                    challengeCreated = true;
                }
                catch (Exception e)
                {
                    Log.Error(e, "Unable to create challenge");
                }
            }
            else // This has to be a pure challenge request
            {
                ChallengeRequest request = new ChallengeRequest
                {
                    Rated = true,
                    ClockLimit = game.Match.ClockTimeLimit,
                    ClockIncrement = game.Match.ClockIncrement,
                    Color = game.BoardPosition % 2 == 0 ? Color.White : Color.Black,
                    Variant = GameVariant.Standard,
                    Fen = game.CurrentFen,
                    Message = "Your EPC team game with {opponent} is ready: {game}."
                };

                // Jv board colors are swapped
                if (game.BoardPosition > 7)
                {
                    if (request.Color == Color.White)
                    {
                        request.Color = Color.Black;
                    }
                    else
                    {
                        request.Color = Color.White;
                    }
                }

                try
                {
                    var response = await client.Challenges.CreateChallenge(awayUser.LichessId, request);

                    game.ChallengeUrl = response.Challenge.Url;
                    game.ChallengeId = response.Challenge.Id;
                    game.ChallengeJson = JsonConvert.SerializeObject(response);
                    game.IsStarted = true;

                    gameJson.Status = response.Challenge.Status;
                    gameJson.IsStarted = true;

                    challengeCreated = true;
                }
                catch (Exception e)
                {
                    Log.Error(e, "Unable to create challenge");
                }

            }

            if (challengeCreated)
            {
                MatchUpdateViewModel vm = new MatchUpdateViewModel
                {
                    MatchId = game.MatchId,
                    Games = new List<GameJson>(new GameJson[] { gameJson })
                };

                // Issue an immediate match update
                await _hubContext.Clients.Groups("match_" + game.Match.MatchId).SendAsync("UpdateMatches", vm);

                _context.Game.Update(game);
                await _context.SaveChangesAsync();

            }
            else
            {
                Log.Information("Challenge Url was empty");

                // This likely happens if the user AccessToken expires and Refresh fails

                await _signInManager.SignOutAsync();
                return RedirectToAction(nameof(AdminController.Index), "Home");
            }

            return Redirect(game.ChallengeUrl);
        }

    }
}
