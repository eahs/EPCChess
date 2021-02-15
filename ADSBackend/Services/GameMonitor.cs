using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ADSBackend.Data;
using ADSBackend.Hubs;
using ADSBackend.Models;
using ADSBackend.Models.PlayhubViewModels;
using ADSBackend.Util;
using LichessApi;
using LichessApi.Web;
using LichessApi.Web.Api.Challenges.Response;
using LichessApi.Web.Api.Games.Request;
using LichessApi.Web.Entities.Enum;
using LichessApi.Web.Exceptions;
using LichessApi.Web.Util;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ADSBackend.Services
{
    public class GameMonitor : HostedService
    {
        private readonly IServiceProvider _provider;
        //private Dictionary<string, Task> _challengeTasks;
        //private Dictionary<string, CancellationTokenSource> _challengeTokens;

        public GameMonitor(IServiceProvider provider)
        {
            _provider = provider;
            //_challengeTasks = new Dictionary<string, Task>();
        }

        /*
        public async Task TrackChallenge(ApplicationDbContext context, Game game)
        {
            if (_challengeTasks.ContainsKey(game.ChallengeId))
                return;
        }

        protected async Task<EventType> LichessMonitor (Game game)
        {
            LichessApiClient client = new LichessApiClient(game.HomePlayer.User.AccessToken);

            CancellationTokenSource cts = new CancellationTokenSource();
            _challengeTokens.Add(game.ChallengeId, cts);

            await foreach (EventStreamResponse evt in client.Challenges.StreamIncomingEvents(cts.Token))
            {
                if (evt.Type == EventType.ChallengeCanceled ||
                    evt.Type == EventType.ChallengeDeclined)
                    return evt.Type;
            }

        }
        */

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                using (IServiceScope scope = _provider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var config = scope.ServiceProvider.GetRequiredService<IConfiguration> ();
                    var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<GameHub>>();
                    var dataService = scope.ServiceProvider.GetRequiredService<DataService>();

                    try
                    {
                        // Get the API access token
                        var lichessAuthNSection = config.GetSection("Authentication:Lichess");
                        var appToken = lichessAuthNSection["AppToken"];
                        
                        // Create a client that can use the token
                        var client = new LichessApiClient(appToken);

                        // Get games from the last week that have not been completed
                        var matches = await context.Match.Where(m => m.IsVirtual && m.MatchStarted && !m.Completed)
                            .Include(m => m.Games).ThenInclude(g => g.HomePlayer).ThenInclude(p => p.User)
                            .Include(m => m.Games).ThenInclude(g => g.AwayPlayer).ThenInclude(p => p.User)
                            .ToListAsync();

                        if (matches is not null)
                        {
                            foreach (var match in matches)
                            {
                                MatchUpdateViewModel vm = new MatchUpdateViewModel
                                {
                                    MatchId = match.MatchId,
                                    Games = new List<GameJson>()
                                };

                                List<string> gameIds = new List<string>();

                                foreach (var game in match.Games)
                                {
                                    if (!game.Completed && !String.IsNullOrEmpty(game.ChallengeId))
                                    {
                                        gameIds.Add(game.ChallengeId);
                                    }
                                    else if (game.Completed)
                                    {
                                        vm.Games.Add(MapGameToJson(game));
                                    }
                                }

                                if (gameIds.Count > 0)
                                {
                                    CancellationTokenSource cts = new CancellationTokenSource();

                                    ExportGamesByIdsRequest request = new ExportGamesByIdsRequest
                                    {
                                        PgnInJson = true,
                                        Moves = true,
                                        Clocks = true,
                                        Evals = false,
                                        Opening = true
                                    };

                                    try
                                    {
                                        await foreach (LichessApi.Web.Models.Game ligame in client.Games
                                            .ExportGamesByIds(request, gameIds, cts.Token))
                                        {
                                            // Remove this from the list
                                            gameIds.Remove(ligame.Id);

                                            Chess chess = new Chess();

                                            if (!String.IsNullOrEmpty(ligame.Moves))
                                            {
                                                try
                                                {
                                                    chess.loadSAN(ligame.Moves);
                                                }
                                                catch (Exception e)
                                                {
                                                    Log.Error(e, "Error loading SAN {0}", ligame.Moves);
                                                }
                                            }

                                            var game = match.Games.FirstOrDefault(g => g.ChallengeId.Equals(ligame.Id));

                                            if (game != null)
                                            {
                                                string fen = chess.Fen();

                                                game.CurrentFen = fen;
                                                game.LastMove = ligame.LastMoveAt;
                                                game.ChallengeStatus = ligame.Status.ToEnumString();
                                                game.ChallengeMoves = ligame.Moves ?? "";

                                                if (ligame.Status == GameStatus.Timeout ||
                                                    ligame.Status == GameStatus.Aborted)
                                                {
                                                    ResetGame(game);
                                                }
                                                else if (ligame.Status == GameStatus.Started)
                                                {
                                                    game.IsStarted = true;
                                                }
                                                else if (ligame.Status == GameStatus.Draw ||
                                                         ligame.Status == GameStatus.Stalemate)
                                                {
                                                    // Result: 0 = Draw, 1 = Home Win, 2 = Away Win, 3 = Reset
                                                    GameResult result = GameResult.Draw;

                                                    await dataService.UpdateAndLogRatingCalculations(game, result);

                                                    game.Completed = true;
                                                    game.CompletedDate = DateTime.Now;
                                                }
                                                else if (ligame.Status == GameStatus.OutOfTime ||
                                                         ligame.Status == GameStatus.Resign ||
                                                         ligame.Status == GameStatus.Mate)
                                                {
                                                    GameResult result = GameResult.Draw;

                                                    if (game.BoardPosition % 2 == 1)
                                                    {
                                                        // black player is home, white player is away
                                                        result = ligame.Winner.Equals("black")
                                                            ? GameResult.Player1Wins
                                                            : GameResult.Player2Wins;
                                                    }
                                                    else
                                                    {
                                                        // white player is home, black player is away 
                                                        result = ligame.Winner.Equals("white")
                                                            ? GameResult.Player1Wins
                                                            : GameResult.Player2Wins;
                                                    }

                                                    // Everything beyond board 7 has flipped colors
                                                    if (game.BoardPosition > 7)
                                                    {
                                                        if (result == GameResult.Player1Wins)
                                                        {
                                                            result = GameResult.Player2Wins;
                                                        }
                                                        else
                                                        {
                                                            result = GameResult.Player1Wins;
                                                        }
                                                        
                                                    }

                                                    await dataService.UpdateAndLogRatingCalculations(game, result);

                                                    game.Completed = true;
                                                    game.CompletedDate = DateTime.Now;
                                                }
                                                else if (ligame.Status == GameStatus.Cheat)
                                                {
                                                    game.CheatingDetected = true;
                                                }

                                                context.Game.Update(game);
                                                await context.SaveChangesAsync();

                                                // Add game to json output
                                                vm.Games.Add(MapGameToJson(game));
                                            }

                                        }

                                    }
                                    catch (ApiInvalidRequestException e)
                                    {
                                        Log.Error(e, "GameMonitor : Invalid Api request");
                                    }
                                    catch (ApiUnauthorizedException e)
                                    {
                                        Log.Error(e, "GameMonitor : Unauthorized request");
                                    }
                                    catch (ApiRateLimitExceededException e)
                                    {
                                        Log.Error(e, "GameMonitor: All requests are rate limited using various strategies, to ensure the API remains responsive for everyone. Only make one request at a time. If you receive an HTTP response with a 429 status, please wait a full minute before resuming API usage.");
                                        await Task.Delay(60 * 1000);  // Wait 60 seconds due to rate limit exceeded
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Error(e, "GameMonitor : Error occurred");
                                    }


                                    // Are there any games that we didn't get responses back for?
                                    if (gameIds.Count > 0)
                                    {
                                        foreach (string gameId in gameIds)
                                        {
                                            var game = match.Games.FirstOrDefault(g => g.ChallengeId.Equals(gameId));

                                            if (game != null)
                                            {
                                                // This is to check for challenge games that are created but we would normally have to monitor
                                                // by streaming events for every user

                                                // Game is created but we aren't getting status updates on it
                                                var response = await client.Connector.SendRawRequest(new Uri(game.ChallengeUrl), HttpMethod.Get);
                                                var body = response.Body.ToString();

                                                if (body.Contains("Challenge canceled") ||
                                                    body.Contains("Challenge declined"))
                                                {
                                                    ResetGame(game);

                                                    context.Game.Update(game);

                                                    await context.SaveChangesAsync();

                                                    // Add game to json output
                                                    vm.Games.Add(MapGameToJson(game));
                                                }
                                                
                                            }
                                        }
                                    }

                                    await hubContext.Clients.Groups("match_" + match.MatchId).SendAsync("UpdateMatches", vm);
                                    await Task.Delay(500);

                                }

                            }


                        }

                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, ex.Message);

                        // if the DB is not currently connected, wait 15 seconds and try again
                        await Task.Delay(TimeSpan.FromSeconds(15));

                        continue;
                    }
                }

                var task = Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                try
                {
                    await task;
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }
        }

        private void ResetGame(Game game)
        {
            game.IsStarted = false;
            game.Completed = false;
            game.CurrentFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            game.ChallengeUrl = "";
            game.ChallengeId = null;
            game.ChallengeStatus = null;
            game.CheatingDetected = false;
            game.ChallengeMoves = "";
        }

        private GameJson MapGameToJson (Game game)
        {
            GameJson gameJson = new GameJson
            {
                GameId = game.GameId.ToString(),
                MatchId = game.MatchId,
                ChallengeId = game.ChallengeId,
                Fen = game.CurrentFen,
                ChallengeUrl = game.ChallengeUrl,
                Moves = game.ChallengeMoves.Split(" ").ToList(),
                Status = game.ChallengeStatus,
                BlackPlayerId = game.BoardPosition % 2 == 1 ? game.HomePlayer.User.LichessId : game.AwayPlayer.User.LichessId,
                WhitePlayerId = game.BoardPosition % 2 == 0 ? game.HomePlayer.User.LichessId : game.AwayPlayer.User.LichessId,
                LastMoveAt = game.LastMove,
                IsStarted = game.IsStarted,
                Completed = game.Completed,
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

            return gameJson;
        }

    }
}
