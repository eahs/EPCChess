using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models.PlayhubViewModels
{
    public class GameJson
    {
        /// <summary>
        /// Id of game on Lichess
        /// </summary>
        public int MatchId { get; set; }
        public string GameId { get; set; }
        public string ChallengeId { get; set; }
        public string ChallengeUrl { get; set; }

        /// <summary>
        /// Current board position in Fen notation
        /// </summary>
        public string Fen { get; set; }

        /// <summary>
        /// List of moves in SAN notation
        /// </summary>
        public List<String> Moves { get; set; }
        public string Result { get; set; }
        public string Status { get; set; }
        public DateTime LastMoveAt { get; set; }
        public string WhitePlayerId { get; set; }
        public string BlackPlayerId { get; set; }
        public string HomePlayerRating { get; set; }
        public string AwayPlayerRating { get; set; }
        public string HomePoints { get; set; }
        public string AwayPoints { get; set; }
        public bool IsStarted { get; set; } = false;
        public bool Completed { get; set; } = false;
    }
}
