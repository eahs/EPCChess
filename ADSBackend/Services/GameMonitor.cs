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
using LichessApi.Web.Api.Challenges.Response;
using LichessApi.Web.Api.Games.Request;
using LichessApi.Web.Entities.Enum;
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
                                    if (!game.Completed && game.ChallengeId is not null)
                                    {
                                        gameIds.Add(game.ChallengeId);

                                        // Not sure if this is needed
                                        if (game.ChallengeStatus.Equals("aborted"))
                                        {
                                            game.ChallengeId = null;
                                            game.IsStarted = false;
                                            game.ChallengeStatus = "";
                                            game.ChallengeUrl = "";

                                            context.Game.Update(game);
                                            await context.SaveChangesAsync();
                                        }
                                    }
                                    else if (game.Completed)
                                    {
                                        GameJson gameJson = new GameJson
                                        {
                                            GameId = game.GameId.ToString(),
                                            ChallengeId = game.ChallengeId,
                                            Fen = game.CurrentFen,
                                            ChallengeUrl = game.ChallengeUrl,
                                            MatchId = game.MatchId,
                                            Moves = game.ChallengeMoves.Split(" ").ToList(),
                                            Status = game.ChallengeStatus,
                                            IsStarted = game.IsStarted,
                                            Completed = game.Completed,
                                            LastMoveAt = game.LastMove,
                                            BlackPlayerId = game.BoardPosition % 2 == 1 ? game.HomePlayer.User.LichessId : game.AwayPlayer.User.LichessId,
                                            WhitePlayerId = game.BoardPosition % 2 == 0 ? game.HomePlayer.User.LichessId : game.AwayPlayer.User.LichessId,
                                            HomePlayerRating = game.Completed ? $"{game.HomePlayerRatingBefore} -> {game.HomePlayerRatingAfter}" : $"{game.HomePlayerRatingAfter}",
                                            AwayPlayerRating = game.Completed ? $"{game.AwayPlayerRatingBefore} -> {game.AwayPlayerRatingAfter}" : $"{game.AwayPlayerRatingAfter}",
                                            HomePoints = game.HomePoints.ToString(),
                                            AwayPoints = game.AwayPoints.ToString()
                                        };

                                        vm.Games.Add(gameJson);
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

                                    await foreach (LichessApi.Web.Models.Game ligame in client.Games.ExportGamesByIds(request, gameIds, cts.Token))
                                    {
                                        // Remove this from the list
                                        gameIds.Remove(ligame.Id);

                                        Chess chess = new Chess();

                                        if (!String.IsNullOrEmpty(ligame.Moves))
                                        {
                                            chess.loadSAN(ligame.Moves);
                                        }

                                        var game = match.Games.FirstOrDefault(g => g.ChallengeId.Equals(ligame.Id));

                                        if (game != null)
                                        {
                                            string fen = chess.Fen();

                                            game.CurrentFen = fen;
                                            game.LastMove = ligame.LastMoveAt;
                                            game.ChallengeStatus = ligame.Status.ToEnumString();
                                            game.ChallengeMoves = ligame.Moves ?? "";

                                            // Result: 0 = Draw, 1 = Home Win, 2 = Away Win, 3 = Reset

                                            if (ligame.Status == GameStatus.Timeout ||
                                                ligame.Status == GameStatus.Aborted)
                                            {
                                                game.IsStarted = false;
                                                game.CurrentFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
                                                game.ChallengeUrl = "";
                                                game.ChallengeId = null;
                                                game.ChallengeStatus = null;
                                                game.CheatingDetected = false;
                                            }
                                            else if (ligame.Status == GameStatus.Started)
                                            {
                                                game.IsStarted = true;
                                            }
                                            else if (ligame.Status == GameStatus.Draw ||
                                                     ligame.Status == GameStatus.Stalemate)
                                            {
                                                GameResult result = GameResult.Draw;

                                                await dataService.UpdateAndLogRatingCalculations(game, result);
                                            }
                                            else if (ligame.Status == GameStatus.OutOfTime ||
                                                     ligame.Status == GameStatus.Resign ||
                                                     ligame.Status == GameStatus.Mate)
                                            {
                                                GameResult result = GameResult.Draw;

                                                if (game.BoardPosition % 2 == 1)
                                                {
                                                    // black player is home, white player is away
                                                    result = ligame.Winner.Equals("black") ? GameResult.Player1Wins : GameResult.Player2Wins;
                                                }
                                                else
                                                {
                                                    // white player is home, black player is away 
                                                    result = ligame.Winner.Equals("white") ? GameResult.Player1Wins : GameResult.Player2Wins;
                                                }

                                                await dataService.UpdateAndLogRatingCalculations(game, result);
                                            }
                                            else if (ligame.Status == GameStatus.Cheat)
                                            {
                                                game.CheatingDetected = true;
                                            }

                                            context.Game.Update(game);
                                            await context.SaveChangesAsync();

                                            GameJson gameJson = new GameJson
                                            {
                                                GameId = game.GameId.ToString(),
                                                ChallengeId = game.ChallengeId,
                                                Fen = fen,
                                                ChallengeUrl = game.ChallengeUrl,
                                                MatchId = game.MatchId,
                                                Moves = ligame.Moves.Split(" ").ToList(),
                                                Status = ligame.Status.ToEnumString(),
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

                                            // Add game to json output
                                            vm.Games.Add(gameJson);
                                        }

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
                                                if (game.ChallengeStatus.Equals("created"))
                                                {
                                                    // Game is created but we aren't getting status updates on it
                                                    var response = await client.Connector.SendRawRequest(new Uri(game.ChallengeUrl), HttpMethod.Get);
                                                    var body = response.Body.ToString();

                                                    if (body.Contains("Challenge canceled") ||
                                                        body.Contains("Challenge declined"))
                                                    {
                                                        game.IsStarted = false;
                                                        game.Completed = false;
                                                        game.CurrentFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
                                                        game.ChallengeUrl = "";
                                                        game.ChallengeId = null;
                                                        game.ChallengeStatus = null;
                                                        game.CheatingDetected = false;

                                                        context.Game.Update(game);

                                                        GameJson gameJson = new GameJson
                                                        {
                                                            GameId = game.GameId.ToString(),
                                                            ChallengeId = game.ChallengeId,
                                                            Fen = game.CurrentFen,
                                                            ChallengeUrl = game.ChallengeUrl,
                                                            MatchId = game.MatchId,
                                                            Moves = new List<string>(),
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

                                                        // Add game to json output
                                                        vm.Games.Add(gameJson);
                                                    }
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

    }
}
