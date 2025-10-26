
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models.PlayhubViewModels
{
    /// <summary>
    /// Represents a game's state in JSON format for client-side updates.
    /// </summary>
    public class GameJson
    {
        /// <summary>
        /// Gets or sets the ID of the match this game belongs to.
        /// </summary>
        public int MatchId { get; set; }
        /// <summary>
        /// Gets or sets the ID of the game.
        /// </summary>
        public string GameId { get; set; }
        /// <summary>
        /// Gets or sets the ID of the challenge on Lichess.
        /// </summary>
        public string ChallengeId { get; set; }
        /// <summary>
        /// Gets or sets the URL of the challenge on Lichess.
        /// </summary>
        public string ChallengeUrl { get; set; }

        /// <summary>
        /// Gets or sets the current board position in FEN notation.
        /// </summary>
        public string Fen { get; set; }

        /// <summary>
        /// Gets or sets the list of moves in SAN notation.
        /// </summary>
        public List<String> Moves { get; set; }
        /// <summary>
        /// Gets or sets the result of the game.
        /// </summary>
        public string Result { get; set; }
        /// <summary>
        /// Gets or sets the status of the game.
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// Gets or sets the timestamp of the last move.
        /// </summary>
        public DateTime LastMoveAt { get; set; }
        /// <summary>
        /// Gets or sets the Lichess ID of the white player.
        /// </summary>
        public string WhitePlayerId { get; set; }
        /// <summary>
        /// Gets or sets the Lichess ID of the black player.
        /// </summary>
        public string BlackPlayerId { get; set; }
        /// <summary>
        /// Gets or sets the rating change for the home player.
        /// </summary>
        public string HomePlayerRating { get; set; }
        /// <summary>
        /// Gets or sets the rating change for the away player.
        /// </summary>
        public string AwayPlayerRating { get; set; }
        /// <summary>
        /// Gets or sets the points awarded to the home player.
        /// </summary>
        public string HomePoints { get; set; }
        /// <summary>
        /// Gets or sets the points awarded to the away player.
        /// </summary>
        public string AwayPoints { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the game has started.
        /// </summary>
        public bool IsStarted { get; set; } = false;
        /// <summary>
        /// Gets or sets a value indicating whether the game is completed.
        /// </summary>
        public bool Completed { get; set; } = false;
    }
}