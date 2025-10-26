
using LichessApi.Web.Api.Challenges.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
    /// <summary>
    /// Represents a single game within a match.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Gets or sets the ID of the game.
        /// </summary>
        [Key]
        public int GameId { get; set; }

        /// <summary>
        /// Gets or sets the board position number.
        /// </summary>
        public int BoardPosition { get; set; }

        /// <summary>
        /// Gets or sets the ID of the match this game belongs to.
        /// </summary>
        public int MatchId { get; set; }
        /// <summary>
        /// Gets or sets the match this game belongs to.
        /// </summary>
        public Match Match { get; set; }

        /// <summary>
        /// Gets or sets the ID of the home player.
        /// </summary>
        public int? HomePlayerId { get; set; }
        /// <summary>
        /// Gets or sets the home player.
        /// </summary>
        public Player HomePlayer { get; set; }

        /// <summary>
        /// Gets or sets the ID of the away player.
        /// </summary>
        public int? AwayPlayerId { get; set; }
        /// <summary>
        /// Gets or sets the away player.
        /// </summary>
        public Player AwayPlayer { get; set; }

        /// <summary>
        /// Gets or sets the home player's rating before the game.
        /// </summary>
        public int HomePlayerRatingBefore { get; set; }
        /// <summary>
        /// Gets or sets the away player's rating before the game.
        /// </summary>
        public int AwayPlayerRatingBefore { get; set; }
        /// <summary>
        /// Gets or sets the home player's rating after the game.
        /// </summary>
        public int HomePlayerRatingAfter { get; set; }
        /// <summary>
        /// Gets or sets the away player's rating after the game.
        /// </summary>
        public int AwayPlayerRatingAfter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the game is completed.
        /// </summary>
        public bool Completed { get; set; }
        /// <summary>
        /// Gets or sets the date and time the game was completed.
        /// </summary>
        public DateTime CompletedDate { get; set; }

        /// <summary>
        /// Gets or sets the points awarded to the home player.
        /// </summary>
        public double HomePoints { get; set; }
        /// <summary>
        /// Gets or sets the points awarded to the away player.
        /// </summary>
        public double AwayPoints { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the game has started.
        /// </summary>
        public bool IsStarted { get; set; } = false;
        /// <summary>
        /// Gets or sets the JSON response for the challenge from Lichess.
        /// </summary>
        public string ChallengeJson { get; set; } = "{}";  // Json response for challenge
        /// <summary>
        /// Gets or sets the ID of the game on Lichess.
        /// </summary>
        public string ChallengeId { get; set; }  // Id of Game
        /// <summary>
        /// Gets or sets the URL of the game on Lichess.
        /// </summary>
        public string ChallengeUrl { get; set; } // Url of Game on Lichess
        /// <summary>
        /// Gets or sets the JSON representation of the game state from Lichess.
        /// </summary>
        public string GameJson { get; set; } = "{}";
        /// <summary>
        /// Gets or sets the current board position in Forsyth-Edwards Notation (FEN).
        /// </summary>
        public string CurrentFen { get; set; } = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        /// <summary>
        /// Gets or sets the timestamp of the last move.
        /// </summary>
        public DateTime LastMove { get; set; }
        /// <summary>
        /// Gets or sets the last time the game data was exported from Lichess.
        /// </summary>
        public DateTime LastGameExportTime { get; set; } = DateTime.Now.AddDays(-5);
        /// <summary>
        /// Gets or sets a value indicating whether cheating was detected in the game.
        /// </summary>
        public bool CheatingDetected { get; set; } = false;
        /// <summary>
        /// Gets or sets the status of the challenge on Lichess.
        /// </summary>
        public string ChallengeStatus { get; set; } = "";
        /// <summary>
        /// Gets or sets the moves of the game in Standard Algebraic Notation (SAN).
        /// </summary>
        public string ChallengeMoves { get; set; } = "";

        /// <summary>
        /// Gets or sets the Lichess API object representation of the challenge.
        /// </summary>
        [NotMapped]
        public ChallengeResponse LiChallenge { get; set; }  // Lichess Api object representation of ChallengeJson

        /// <summary>
        /// Gets or sets the Lichess API object representation of the game.
        /// </summary>
        [NotMapped]
        public LichessApi.Web.Models.Game LiGame { get; set; }  // Lichess Api object representation of GameJson

        /// <summary>
        /// Gets the full name of the home player.
        /// </summary>
        [NotMapped]
        public string HomePlayerFullName => $"{HomePlayer?.FirstName} {HomePlayer?.LastName}";

        /// <summary>
        /// Gets the full name of the away player.
        /// </summary>
        [NotMapped]
        public string AwayPlayerFullName => $"{AwayPlayer?.FirstName} {AwayPlayer?.LastName}";

        /// <summary>
        /// Gets the rating change display string for the home player.
        /// </summary>
        [NotMapped]
        public string HomePlayerGameRating
        {
            get
            {
                if (HomePlayer != null)
                {
                    if ((HomePoints+AwayPoints) == 0)
                        return $"({HomePlayerRatingBefore})";
                    else
                        return $"({HomePlayerRatingBefore} -> {HomePlayerRatingAfter})";
                }

                return "";
            }
        }

        /// <summary>
        /// Gets the rating change display string for the away player.
        /// </summary>
        [NotMapped]
        public string AwayPlayerGameRating
        {
            get
            {
                if (AwayPlayer != null)
                {
                    if ((HomePoints + AwayPoints) == 0)
                        return $"({AwayPlayerRatingBefore})";
                    else
                        return $"({AwayPlayerRatingBefore} -> {AwayPlayerRatingAfter})";
                }

                return "";
            }
        }


    }
}